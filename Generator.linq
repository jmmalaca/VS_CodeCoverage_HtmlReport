<Query Kind="Program" />

void Main()
{
	var CodeCoverageFilePath = @"< path to the .coveragexml file >";
	var CodeCoverageHtmlPath = @"< path to the folder where the html file will be created >";
	
	//Read and Generate Data
	var AddModulesData = true;
	var AddNamespacesData = false;
	var AddClassesData = true;
	var AddMethodsData = false;
	var coverageData = new CodeCoverageDataReader(CodeCoverageFilePath, AddModulesData, AddNamespacesData, AddClassesData, AddMethodsData);
	
	//Print Data on Html
	var htmlReporter = new HtmlReportWriter(coverageData, CodeCoverageHtmlPath);
	htmlReporter.PrintModulesHtmlFile("CodeCoverageData.html");
}

public class HtmlReportWriter{

	#region Fields
	
	private CodeCoverageDataReader coverageData;
	private string projectPath;
	private string HtmlStart = "<!DOCTYPE html>" + 
		"<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
		"<head>" +
        "<title>Code Coverage Report</title>" +
        "<meta charset=\"utf-8\" />" +
        "<meta name=\"author\" content=\"Jose Miguel Malaca\" />" +
        "<meta name=\"description\" content=\"Code Coverage Report\" />" +
        "<meta name=\"keywords\" content=\"code,coverage,report,codecoveragereport,visual,studio,visualstudio\"/>" +
		"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
		"</head>" + 
		"<body>";
	private string HtmlTitle;
	private string HtmlEnd = "</body>" +
		"</html>";
	private string ModulesHtmlTableStartCode = "<table style=\"width:100%; border: 1px solid black;\">";
	private string ModulesHtmlTableHeaders = "<tr>" +
		"<th>Name</th>" +
		"<th>Blocks Covered</th>" +
		"<th>Blocks Not Covered</th>" +
		"<th>Blocks Covered (%)</th>" +
		"<th>Blocks Not Covered (%)</th>" +
		"</tr>";
	private string ModulesHtmlTableEndCode = "</table>";
	
	#endregion

	#region constructor
	
	public HtmlReportWriter(CodeCoverageDataReader data, string path){
		this.coverageData = data;
		this.projectPath = path;
	}
	
	#endregion
	
	#region Methods
	
	private string GenerateModulesHtmlTableCode(){
		return this.ModulesHtmlTableStartCode +
			this.ModulesHtmlTableHeaders +
			this.coverageData.ModulesHtmlTableContent +
			this.ModulesHtmlTableEndCode;
	}
	
	public void PrintModulesHtmlFile(string fileName){
	
		var modulesHtmlTableContent = GenerateModulesHtmlTableCode();
		this.HtmlTitle = "<h1 style=\"text-align:center\"> " + this.coverageData.fileName + " </h1><br><br>";
		
		using (FileStream fs = new FileStream(projectPath + fileName, FileMode.Create)) 
		{ 
			using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8)) 
			{ 
				w.WriteLine(HtmlStart);
				w.WriteLine(HtmlTitle);
				w.WriteLine(modulesHtmlTableContent);
				w.WriteLine(HtmlEnd);
			} 
		} 
	}
	
	#endregion
}

public class CodeCoverageDataReader{
	
	#region Fields
	
	private bool AddModulesRowsData = false;
	private bool AddNamespacesRowsData = false;
	private bool AddClassesRowsData = false;
	private bool AddMethodsRowsData = false;
	
	#endregion
	
	#region constructor
	
	public CodeCoverageDataReader(string reportPath, bool AddModules, bool AddNamespaces, bool AddClasses, bool AddMethods){
		this.AddModulesRowsData = AddModules;
		this.AddNamespacesRowsData = AddNamespaces;
		this.AddClassesRowsData = AddClasses;
		this.AddMethodsRowsData = AddMethods;
		ReadCoverageData(reportPath);
	}
	
	#endregion
	
	#region Properties
	
	//File Name
	public string fileName{
		get;
		set;
	}
	
	//Modules
	public List<ModuleData> ModulesData{
		get;
		set;
	}
	
	//html Modules Table Code
	public string ModulesHtmlTableContent{
		get;
		set;
	}
	
	#endregion
	
	#region Methods
	
	private void ReadCoverageData(string filePath){
		var pathData = filePath.Split('\\');
		this.fileName = pathData[pathData.Length-1];
		
		ModulesData = new List<ModuleData>();
		var doc = XDocument.Load(filePath);
		var filesData = GetFilesData(doc);
		
		doc.Descendants("Module").ToList().ForEach(module =>{
			var newModule = new ModuleData(module, filesData, AddModulesRowsData, AddNamespacesRowsData, AddClassesRowsData, AddMethodsRowsData);
			ModulesHtmlTableContent += newModule.HtmlTableRowCode;
			ModulesData.Add(newModule);
		});
	}
	
	private List<FileData> GetFilesData(XDocument doc){
		var dataList = new List<FileData>();
		var sourceFilesObjs = doc.Descendants("SourceFileNames");
		sourceFilesObjs.ToList().ForEach(item =>
		{
			var fileData = new FileData();
			fileData.Id = item.Element("SourceFileID").Value;
			var filePath = item.Element("SourceFileName").Value;
			var pathParts = filePath.Split('\\');
			fileData.Name = pathParts[pathParts.Length - 1];
			fileData.Path = filePath.Replace(fileData.Name,"");
			dataList.Add(fileData);
		});
		return dataList;
	}
	
	#endregion
}

public class ModuleData{
	
	#region Fields
	
	private bool AddModulesRowsData = false;
	private bool AddNamespacesRowsData = false;
	private bool AddClassesRowsData = false;
	private bool AddMethodsRowsData = false;
	private string HtmlBlackDot = "&#8226;";
	
	#endregion
	
	#region constructor
	
	public ModuleData(XElement module, List<FileData> filesData, bool AddModules, bool AddNamespaces, bool AddClasses, bool AddMethods){
		this.AddModulesRowsData = AddModules;
		this.AddNamespacesRowsData = AddNamespaces;
		this.AddClassesRowsData = AddClasses;
		this.AddMethodsRowsData = AddMethods;
		this.GetModuleData(module, filesData);
		this.GenerateHtmlTableRowCode();
	}
	
	#endregion
	
	#region Properties
	
	//	<ModuleName>sofly.analytics.storage.dll</ModuleName>
	public string ModuleName
	{
		get;
		set;
    }
    
	//  <ImageSize>204800</ImageSize>
	public string ImageSize
	{
		get;
		set;
    }
    
	//  <ImageLinkTime>0</ImageLinkTime>
	public string ImageLinkTime
	{
		get;
		set;
    }
    
	//  <LinesCovered>533</LinesCovered>
	public string LinesCovered
	{
		get;
		set;
    }
    
	//  <LinesPartiallyCovered>13</LinesPartiallyCovered>
	public string LinesPartiallyCovered
	{
		get;
		set;
    }
    
	//  <LinesNotCovered>254</LinesNotCovered>
	public string LinesNotCovered
	{
		get;
		set;
    }
    
	//  <BlocksCovered>572</BlocksCovered>
	public string BlocksCovered
	{
		get;
		set;
    }
    
	public string BlocksCoveredPercentage
	{
		get;
		set;
    }
	
	//  <BlocksNotCovered>237</BlocksNotCovered>
	public string BlocksNotCovered
	{
		get;
		set;
    }
	
	public string BlocksNotCoveredPercentage
	{
		get;
		set;
    }
    
	//  <NamespaceTable>
	public List<NamespaceTableData> NamespacesTableData
	{
		get;
		set;
    }
	
	//Html Table Row Code
	public string HtmlTableRowCode
	{
		get;
		set;
    }
	
	#endregion
	
	#region Methods
	
	private void GetModuleData(XElement module, List<FileData> filesData){
		this.ModuleName = module.Element("ModuleName").Value;
		this.ImageSize = module.Element("ImageSize").Value;
		this.ImageLinkTime = module.Element("ImageLinkTime").Value;
		this.LinesCovered = module.Element("LinesCovered").Value;
		this.LinesPartiallyCovered = module.Element("LinesPartiallyCovered").Value;
		this.LinesNotCovered = module.Element("LinesNotCovered").Value;
		this.BlocksCovered = module.Element("BlocksCovered").Value;
		this.BlocksNotCovered = module.Element("BlocksNotCovered").Value;
		
		var totalBlocks = double.Parse(this.BlocksCovered) + double.Parse(this.BlocksNotCovered);
		this.BlocksCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksCovered) * 100) / (double)totalBlocks),2));
		this.BlocksNotCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksNotCovered) * 100) / (double)totalBlocks),2));
		
		this.NamespacesTableData = new List<NamespaceTableData>();
		module.Descendants("NamespaceTable").ToList().ForEach(namespaceT =>{
			this.NamespacesTableData.Add(new NamespaceTableData(namespaceT, filesData, this.AddNamespacesRowsData, this.AddClassesRowsData, this.AddMethodsRowsData));
		});
	}
	
	private void GenerateHtmlTableRowCode(){
		if (AddModulesRowsData){
			this.HtmlTableRowCode += "<tr>" +
				"<td style=\"background:#66A3C2; text-align: left;\">" + this.HtmlBlackDot + "Module: " + this.ModuleName + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksCovered + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksNotCovered + "</td>" +
				"<td style=\"background:#66E066; text-align: center;\">" + this.BlocksCoveredPercentage + "</td>" +
				"<td style=\"background:#FF4D4D; text-align: center;\">" + this.BlocksNotCoveredPercentage + "</td>" +
				"</tr>";
		}
		this.NamespacesTableData.ForEach(item => {
			this.HtmlTableRowCode += item.HtmlTableRowCode;
		});
	}
	
	#endregion
}

public class NamespaceTableData{
	
	#region Fields
	
	private bool AddNamespacesRowsData = false;
	private bool AddClassesRowsData = false;
	private bool AddMethodsRowsData = false;
	private string Spaces = "&nbsp;&nbsp;&nbsp;";
	private string HtmlBlackDot = "&#8226;";
	
	#endregion
	
	#region constructor
	
	public NamespaceTableData(XElement namespaceT, List<FileData> filesData, bool addNamespaces, bool addClassesData, bool addMethodsData){
		this.AddNamespacesRowsData = addNamespaces;
		this.AddClassesRowsData = addClassesData;
		this.AddMethodsRowsData = addMethodsData;
		this.GetNamespaceTableData(namespaceT, filesData);
		this.GenerateHtmlTableRowCode();
	}
	
	#endregion
	
	#region Properties
	
	//  <BlocksCovered>347</BlocksCovered>
	public string BlocksCovered
	{
		get;
		set;
    }
    
	public string BlocksCoveredPercentage
	{
		get;
		set;
    }
	
    //  <BlocksNotCovered>67</BlocksNotCovered>
	public string BlocksNotCovered
	{
		get;
		set;
    }
    
	public string BlocksNotCoveredPercentage
	{
		get;
		set;
    }
	
    //  <LinesCovered>349</LinesCovered>
	public string LinesCovered
	{
		get;
		set;
    }
    
    //  <LinesNotCovered>77</LinesNotCovered>
	public string LinesNotCovered
	{
		get;
		set;
    }
    
    //  <LinesPartiallyCovered>6</LinesPartiallyCovered>
	public string LinesPartiallyCovered
	{
		get;
		set;
    }
    
    //  <ModuleName>sofly.analytics.storage.dll</ModuleName>
	public string ModuleName
	{
		get;
		set;
    }
    
    //  <NamespaceKeyName>sofly.analytics.storage.dll8d4d647e-d8a5-af40-a2ae-8697e0df8006SoFly.Analytics.Storage</NamespaceKeyName>
	public string NamespaceKeyName
	{
		get;
		set;
    }
    
    //  <NamespaceName>SoFly.Analytics.Storage</NamespaceName>
	public string NamespaceName
	{
		get;
		set;
    }
    
	//  Class
	public List<ClassData> ClassesData
	{
		get;
		set;
    }
	
	//Html Table Row Code
	public string HtmlTableRowCode
	{
		get;
		set;
    }
	
	#endregion
	
	#region Methods
	
	private void GetNamespaceTableData(XElement namespaceT, List<FileData> filesData){
		this.BlocksCovered = namespaceT.Element("BlocksCovered").Value;
		this.BlocksNotCovered = namespaceT.Element("BlocksNotCovered").Value;
		
		var totalBlocks = double.Parse(this.BlocksCovered) + double.Parse(this.BlocksNotCovered);
		this.BlocksCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksCovered) * 100) / (double)totalBlocks),2));
		this.BlocksNotCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksNotCovered) * 100) / (double)totalBlocks),2));
		
		this.LinesCovered = namespaceT.Element("LinesCovered").Value;
		this.LinesNotCovered = namespaceT.Element("LinesNotCovered").Value;
		this.LinesPartiallyCovered = namespaceT.Element("LinesPartiallyCovered").Value;
		this.ModuleName = namespaceT.Element("ModuleName").Value;
		this.NamespaceKeyName = namespaceT.Element("NamespaceKeyName").Value;
		this.NamespaceName = namespaceT.Element("NamespaceName").Value;
		
		this.ClassesData = new List<ClassData>();
		namespaceT.Descendants("Class").ToList().ForEach(classe =>{
			this.ClassesData.Add(new ClassData(classe, filesData, this.AddClassesRowsData, this.AddMethodsRowsData));
		});
	}
	
	private void GenerateHtmlTableRowCode(){
		if (this.AddNamespacesRowsData){
			this.HtmlTableRowCode = "<tr>" +
				"<td style=\"background:#80B2CC; text-align: left;\">" + this.Spaces + this.HtmlBlackDot + "Namespace: " +  this.NamespaceName + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksCovered + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksNotCovered + "</td>" +
				"<td style=\"background:#66E066; text-align: center;\">" + this.BlocksCoveredPercentage + "</td>" +
				"<td style=\"background:#FF4D4D; text-align: center;\">" + this.BlocksNotCoveredPercentage + "</td>" +
				"</tr>";
		}
		this.ClassesData.ForEach(item => {
			this.HtmlTableRowCode += item.HtmlTableRowCode;
		});
	}
	
	#endregion
}

public class ClassData{
	
	#region Fields
	
	private bool AddClassesRowsData = false;
	private bool AddMethodsRowsData = false;
	private string Spaces = "&nbsp;&nbsp;&nbsp;";
	private string HtmlBlackDot = "&#8226;";
	
	#endregion
	
	#region constructor
	
	public ClassData(XElement classe, List<FileData> filesData, bool addClasses, bool addMethodsData){
		this.AddClassesRowsData = addClasses;
		this.AddMethodsRowsData = addMethodsData;
		this.GetClassData(classe, filesData);
		this.GenerateHtmlTableRowCode();
	}
	
	#endregion
	
	#region Properties
	
	//  <ClassKeyName>sofly.analytics.storage.dll8d4d647e-d8a5-af40-a2ae-8697e0df8006SoFly.Analytics.StorageClientContext</ClassKeyName>
	public string ClassKeyName
	{
		get;
		set;
    }
    
    //  <ClassName>ClientContext</ClassName>
	public string ClassName
	{
		get;
		set;
    }
    
    //  <LinesCovered>127</LinesCovered>
	public string LinesCovered
	{
		get;
		set;
    }
    
    //  <LinesNotCovered>33</LinesNotCovered>
	public string LinesNotCovered
	{
		get;
		set;
    }
    
    //  <LinesPartiallyCovered>3</LinesPartiallyCovered>
	public string LinesPartiallyCovered
	{
		get;
		set;
    }
    
    //  <BlocksCovered>123</BlocksCovered>
	public string BlocksCovered
	{
		get;
		set;
    }
    
	public string BlocksCoveredPercentage
	{
		get;
		set;
    }
	
    //  <BlocksNotCovered>29</BlocksNotCovered>
	public string BlocksNotCovered
	{
		get;
		set;
    }
    
	public string BlocksNotCoveredPercentage
	{
		get;
		set;
    }
	
    //  <NamespaceKeyName>sofly.analytics.storage.dll8d4d647e-d8a5-af40-a2ae-8697e0df8006SoFly.Analytics.Storage</NamespaceKeyName>
	public string NamespaceKeyName
	{
		get;
		set;
    }
    
	//  <Method>
	public List<MethodData> MethodsData
	{
		get;
		set;
    }
	
	//Html Table Row Code
	public string HtmlTableRowCode
	{
		get;
		set;
    }
	
	#endregion
	
	#region Methods
	
	private void GetClassData(XElement classe, List<FileData> filesData){
		this.ClassKeyName = classe.Element("ClassKeyName").Value;
		this.ClassName = classe.Element("ClassName").Value;
		this.LinesCovered = classe.Element("LinesCovered").Value;
		this.LinesNotCovered = classe.Element("LinesNotCovered").Value;
		this.LinesPartiallyCovered = classe.Element("LinesPartiallyCovered").Value;
		this.BlocksCovered = classe.Element("BlocksCovered").Value;
		this.BlocksNotCovered = classe.Element("BlocksNotCovered").Value;
		
		var totalBlocks = double.Parse(this.BlocksCovered) + double.Parse(this.BlocksNotCovered);
		this.BlocksCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksCovered) * 100) / (double)totalBlocks),2));
		this.BlocksNotCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksNotCovered) * 100) / (double)totalBlocks),2));
		
		this.NamespaceKeyName = classe.Element("NamespaceKeyName").Value;
		
		this.MethodsData = new List<MethodData>();
		classe.Descendants("Method").ToList().ForEach(method =>{
			this.MethodsData.Add(new MethodData(method, filesData, this.AddMethodsRowsData));
		});
	}
	
	private void GenerateHtmlTableRowCode(){
		if (this.AddClassesRowsData){
			this.HtmlTableRowCode = "<tr>" +
				"<td style=\"background:#99C2D6; text-align: left;\">" + this.Spaces + this.Spaces + this.HtmlBlackDot + "Class: " + this.ClassName + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksCovered + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksNotCovered + "</td>" +
				"<td style=\"background:#66E066; text-align: center;\">" + this.BlocksCoveredPercentage + "</td>" +
				"<td style=\"background:#FF4D4D; text-align: center;\">" + this.BlocksNotCoveredPercentage + "</td>" +
				"</tr>";
		}
		this.MethodsData.ForEach(item => {
			this.HtmlTableRowCode += item.HtmlTableRowCode;
		});
	}
	
	#endregion
}

public class MethodData{
	
	#region Fields
	
	private bool AddMethodsRowsData = false;
	private string Spaces = "&nbsp;&nbsp;&nbsp;";
	private string HtmlBlackDot = "&#8226;";
	
	#endregion
	
	#region constructor
	
	public MethodData(XElement method, List<FileData> filesData, bool AddMethods){
		this.AddMethodsRowsData = AddMethods;
		this.GetMethodData(method, filesData);
		this.GenerateHtmlTableRowCode();
	}
	
	#endregion
	
	#region Properties
	
	//  <MethodKeyName>sofly.analytics.storage.dll8d4d647e-d8a5-af40-a2ae-8697e0df8006SoFly.Analytics.StorageClientContext!ClientContext(string, int, string, string, string)!8272</MethodKeyName>
	public string MethodKeyName
	{
		get;
		set;
    }
	
   	//  <MethodName>ClientContext(string, int, string, string, string)</MethodName>
	public string MethodName
	{
		get;
		set;
    }
	
   	//  <MethodFullName>ClientContext(string, int, string, string, string)</MethodFullName>
	public string MethodFullName
	{
		get;
		set;
    }
	
   	//  <LinesCovered>3</LinesCovered>
	public string LinesCovered
	{
		get;
		set;
    }
	
   	//  <LinesPartiallyCovered>0</LinesPartiallyCovered>
	public string LinesPartiallyCovered
	{
		get;
		set;
    }
	
   	//  <LinesNotCovered>0</LinesNotCovered>
	public string LinesNotCovered
	{
		get;
		set;
    }
	
   	//  <BlocksCovered>2</BlocksCovered>
	public string BlocksCovered
	{
		get;
		set;
    }
	
	public string BlocksCoveredPercentage
	{
		get;
		set;
    }
	
   	//  <BlocksNotCovered>0</BlocksNotCovered>
	public string BlocksNotCovered
	{
		get;
		set;
    }
	
	public string BlocksNotCoveredPercentage
	{
		get;
		set;
    }
	
	//  Lines
	public List<LineData> LinesData
	{
		get;
		set;
    }
	
	//Html Table Row Code
	public string HtmlTableRowCode
	{
		get;
		set;
    }
	
	#endregion
	
	#region Methods
	
	private void GetMethodData(XElement method, List<FileData> filesData){
		this.MethodKeyName = method.Element("MethodKeyName").Value;
		this.MethodName = method.Element("MethodName").Value;
		this.MethodFullName = method.Element("MethodFullName").Value;
		this.LinesCovered = method.Element("LinesCovered").Value;
		this.LinesPartiallyCovered = method.Element("LinesPartiallyCovered").Value;
		this.LinesNotCovered = method.Element("LinesNotCovered").Value;
		this.BlocksCovered = method.Element("BlocksCovered").Value;
		this.BlocksNotCovered = method.Element("BlocksNotCovered").Value;
		
		var totalBlocks = double.Parse(this.BlocksCovered) + double.Parse(this.BlocksNotCovered);
		this.BlocksCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksCovered) * 100) / (double)totalBlocks),2));
		this.BlocksNotCoveredPercentage = string.Format("{0}%",Math.Round(((double.Parse(this.BlocksNotCovered) * 100) / (double)totalBlocks),2));
		
		this.LinesData = new List<LineData>();
		method.Descendants("Lines").ToList().ForEach(line =>{
			this.LinesData.Add(new LineData(line, filesData));
		});
	}
	
	private void GenerateHtmlTableRowCode(){
		if(this.AddMethodsRowsData){
			this.HtmlTableRowCode = "<tr>" +
				"<td style=\"background:#B2D1E0; text-align: left;\">" + this.Spaces + this.Spaces + this.Spaces + this.HtmlBlackDot + "Method: " + this.MethodName + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksCovered + "</td>" +
				"<td style=\"background:#CCCCCC; text-align: center;\">" + this.BlocksNotCovered + "</td>" +
				"<td style=\"background:#66E066; text-align: center;\">" + this.BlocksCoveredPercentage + "</td>" +
				"<td style=\"background:#FF4D4D; text-align: center;\">" + this.BlocksNotCoveredPercentage + "</td>" +
				"</tr>";
		}
	}
	
	#endregion
}

public class LineData{
	
	#region constructor
	
	public LineData(XElement line, List<FileData> filesData){
		this.GetLineData(line, filesData);
	}
	
	#endregion
	
	#region Properties
	
	//  <LnStart>45</LnStart>
	public string LnStart
	{
		get;
		set;
    }
	
	//  <ColStart>9</ColStart>
	public string ColStart
	{
		get;
		set;
    }
	
	//  <LnEnd>46</LnEnd>
	public string LnEnd
	{
		get;
		set;
    }
	
	//  <ColEnd>64</ColEnd>
	public string ColEnd
	{
		get;
		set;
    }
	
	//  <Coverage>0</Coverage>
	public string Coverage
	{
		get;
		set;
    }
	
	//  <SourceFileID>1</SourceFileID> && SourceFileData
	public string SourceFileID
	{
		get;
		set;
    }
	
	//  < File Data >
	public FileData SourceFileData
	{
		get;
		set;
	}
	
	//  <LineID>0</LineID>
	public string LineID
	{
		get;
		set;
    }
	
	#endregion
	
	#region Methods
	
	private void GetLineData(XElement line, List<FileData> filesData){
		this.LnStart = line.Element("LnStart").Value;
		this.ColStart = line.Element("ColStart").Value;
		this.LnEnd = line.Element("LnEnd").Value;
		this.ColEnd = line.Element("ColEnd").Value;
		this.Coverage = line.Element("Coverage").Value;
		this.SourceFileID = line.Element("SourceFileID").Value;
		this.SourceFileData = filesData.FirstOrDefault(fileD => fileD.Id.Equals(this.SourceFileID));
		this.LineID = line.Element("LineID").Value;
	}
	
	#endregion
}

public class FileData{

	#region Properties
	
	//Id
	public string Id
	{
		get;
		set;
    }
	
	//Name
	public string Name
	{
		get;
		set;
    }
	
	//Path
	public string Path
	{
		get;
		set;
    }
	
	#endregion
	
	#region Methods
	#endregion
}