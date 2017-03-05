#addin "Cake.Yaml"
#addin "Newtonsoft.Json"
var target = Argument("target", "Local");
var os = Argument("os","Windows2016StdCore");
bool installVBoxTools = Argument<bool>("install_vbox_tools", true);

// load build config yaml
var OSES = LoadYAMLConfig("./build.config.yaml", os);

using Newtonsoft.Json;

public void DebugPrint(object obj)
{
    if(obj == null)
        Information("{0}", "obj is null");

    var objAsJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
    Information("{0}", objAsJson);
}

public class OSToBuild
{
    public string Name { get; set; }
    public string osName { get; set; }
    public string guestOSType { get; set; }
    public string isoURL { get; set; }
    public string isoChecksum { get; set; }
}

public OSToBuild LoadYAMLConfig(string yaml_path, string os)
{
    //OSToBuild os_to_build_properties;
    try
    {
        var oses = DeserializeYamlFromFile<List<OSToBuild>> (yaml_path);

        // check if the OS the user passed exists
        bool matchingOS = oses.Any(n => n.Name == os);

        if (matchingOS == true)
        {
            // return the matching os to build
            return oses.Where(n => n.Name == os).First();
        }
        else
        {
            string exceptionMsg = string.Format("Could not find a matching operating system in {0}. You passed in: {1}", yaml_path, os);
            throw new System.ArgumentException(exceptionMsg);
        }
    }
    catch
    {
        throw new System.ArgumentException("Your yaml is invalid.");
    }
}


public string GetPackerSourcePath(string os_name, string source_path)
{
    string source_path_var = String.Format(source_path,
      os_name
    );

    Information("Source Path: " + source_path_var);

    return source_path_var;
}

public ProcessSettings RunPacker(bool install_vbox_tools, OSToBuild os, string source_path, string json_file_path)
{
  string vbox_tools;
  string source_path_var;

  if (install_vbox_tools)
  {
    vbox_tools = "True";
  }
  else
  {
    vbox_tools = "False";
  }

  if (source_path != null)
  {
    source_path_var = String.Format(" -var \"source_path={0}\"",
      GetPackerSourcePath(os.osName, source_path)
    );
  }
  else
  {
    source_path_var = "";
  }

  string packer_cmd = String.Format("-var \"install_vbox_tools={0}\" -var \"os_name={1}\" -var \"iso_checksum={2}\" -var \"iso_url={3}\" -var \"guest_os_type={4}\" -var \"full_os_name={5}\" {6} {7}",
    vbox_tools,
    os.osName,
    os.isoChecksum,
    os.isoURL,
    os.guestOSType,
    os.Name,
    source_path_var,
    json_file_path
  );

  Information(packer_cmd);

  var settings = new ProcessSettings
  {
    Arguments = new ProcessArgumentBuilder().Append("build").Append(packer_cmd)
  };
  return settings;
}

Task("01-windows-base")
  .Does(() =>
{
    StartProcess("packer", RunPacker(intallVBoxTools, OSES, "", "01-windows-base.json"));
});

Task("02-win_updates-wmf5")
  .IsDependentOn("01-windows-base")
  .Does(() =>
{
    // string packer_source_path = GetPackerSourcePath(OSES.osName, "./output-{0}-base/{0}-base.ovf");

    StartProcess("packer", RunPacker(intallVBoxTools, OSES, "./output-{0}-base/{0}-base.ovf", "02-win_updates-wmf5.json"));
});

Task("03-cleanup")
  .IsDependentOn("02-win_updates-wmf5")
  .Does(() =>
{
    // string packer_source_path = GetPackerSourcePath(OSES.osName, "./output-{0}-updates_wmf5/{0}-updates_wmf5.ovf");

    StartProcess("packer", RunPacker(intallVBoxTools, OSES, "./output-{0}-updates_wmf5/{0}-updates_wmf5.ovf", "03-cleanup.json"));
});

Task("Local")
  .IsDependentOn("03-cleanup")
  .Does(() =>
{
    // string packer_source_path = GetPackerSourcePath(OSES.osName, "./output-{0}-cleanup/{0}-cleanup.ovf");

    StartProcess("packer", RunPacker(intallVBoxTools, OSES, "./output-{0}-cleanup/{0}-cleanup.ovf", "04-local.json"));
});

Task("Atlas")
  .IsDependentOn("03-cleanup")
  .Does(() =>
{
    // string packer_source_path = GetPackerSourcePath(OSES.osName, "./output-{0}-cleanup/{0}-cleanup.ovf");

    StartProcess("packer", RunPacker(intallVBoxTools, OSES, "./output-{0}-cleanup/{0}-cleanup.ovf", "04-atlas.json"));
});

RunTarget(target);
