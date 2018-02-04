#addin "Cake.Incubator"
#addin "Cake.Yaml"
#addin "YamlDotNet"

// CLI Arguments For Cake Script
var buildTarget = Argument("target", "virtualbox-local");
var os = Argument("os","Windows2016StdCore");

// These need to be environment variables if doing a vagrant cloud build
if (buildTarget.Contains("vagrant-cloud")) {
  EnvironmentVariable<string>("ATLAS_TOKEN");
  EnvironmentVariable<string>("ATLAS_USERNAME");
  EnvironmentVariable<string>("ATLAS_VERSION");
}

string virtualBoxBuilderPath = "builders/virtualbox";
string hypervBuilderPath = "builders/hyperv";

// load build config yaml
var OSES = LoadYAMLConfig("./build.supported_os.yaml", os);

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
            string exceptionMsg = $"Could not find a matching operating system in {yaml_path}. You passed in: {os}";
            throw new System.ArgumentException(exceptionMsg);
        }
    }
    catch
    {
        throw new System.ArgumentException($"Your YAML file at {yaml_path} is invalid!");
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

public ProcessSettings RunPacker(OSToBuild os, string source_path, string json_file_path)
{
  string source_path_var;

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

  string packer_cmd = $"-var \"os_name={os.osName}\" -var \"iso_checksum={os.isoChecksum}\" -var \"iso_url={os.isoURL}\" -var \"guest_os_type={os.guestOSType}\" -var \"full_os_name={os.Name}\" {source_path_var} {json_file_path}";

  Information(packer_cmd);

  var settings = new ProcessSettings
  {
    Arguments = new ProcessArgumentBuilder().Append("build").Append(packer_cmd)
  };
  return settings;
}

// Clean
Task("clean")
  .Does(() =>
{
    var directoriesToClean = GetDirectories("./output-*/**");

    var deleteSettings = new DeleteDirectorySettings {
      Recursive = true,
      Force = true,
    };

    foreach (var directory in directoriesToClean)
    {
        if (DirectoryExists(directory))
        {
            DeleteDirectory(directory, deleteSettings);
            Information($"Deleted directory {directory}.");
        }
    }
});

// VirtualBox Tasks
Task("virtualbox-01-windows-base")
  .Does(() =>
{
    string jsonToBuild = $"{virtualBoxBuilderPath}/01-windows-base.json";
    StartProcess("packer", RunPacker(OSES, "", jsonToBuild));
});

Task("virtualbox-02-win_updates-wmf5")
  .IsDependentOn("virtualbox-01-windows-base")
  .Does(() =>
{
    string jsonToBuild = $"{virtualBoxBuilderPath}/02-win_updates-wmf5.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-base/{0}-base.ovf", jsonToBuild));
});

Task("virtualbox-03-cleanup")
  .IsDependentOn("virtualbox-02-win_updates-wmf5")
  .Does(() =>
{
    string jsonToBuild = $"{virtualBoxBuilderPath}/03-cleanup.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-updates_wmf5/{0}-updates_wmf5.ovf", jsonToBuild));
});

Task("virtualbox-local")
  .IsDependentOn("virtualbox-03-cleanup")
  .Does(() =>
{
    string jsonToBuild = $"{virtualBoxBuilderPath}/04-local.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-cleanup/{0}-cleanup.ovf", jsonToBuild));
});

Task("virtualbox-vagrant-cloud")
  .IsDependentOn("virtualbox-03-cleanup")
  .Does(() =>
{
    string jsonToBuild = $"{virtualBoxBuilderPath}/04-vagrant-cloud.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-cleanup/{0}-cleanup.ovf", jsonToBuild));
});

// Hyper-V Tasks
Task("hyperv-local")
  .Does(() =>
{
    string jsonToBuild = $"{hypervBuilderPath}/01-windows-local.json";
    StartProcess("packer", RunPacker(OSES, "", jsonToBuild));
});

Task("hyperv-vagrant-cloud")
  .Does(() =>
{
    string jsonToBuild = $"{hypervBuilderPath}/01-windows-vagrant-cloud.json";
    StartProcess("packer", RunPacker(OSES, "", jsonToBuild));
});

RunTarget(buildTarget);
