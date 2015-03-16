void Main()
{
	var CodeCoverageFilePath = @"< path to the .coveragexml file >";
	var CodeCoverageHtmlPath = @"< path to the folder where the html file will be created >";
	
	//Read and Generate Data
	var AddModulesData = true;
	var AddNamespacesData = true;
	var AddClassesData = true;
	var AddMethodsData = false;
	var coverageData = new CodeCoverageDataReader(CodeCoverageFilePath, AddModulesData, AddNamespacesData, AddClassesData, AddMethodsData);
	
	//Print Data on Html
	var htmlReporter = new HtmlReportWriter(coverageData, CodeCoverageHtmlPath);
	htmlReporter.Generate();
}

public class HtmlReportWriter{

	#region Fields
	
	private CodeCoverageDataReader CodeCoverageData;
	private string CodeCoverageHtmlPath;
	
	#region Html Global
	
	private string HtmlTitle;
	private string HtmlSpace = "<br><br>";
	private string HtmlEnd = "</body>" +
		"</html>";
	private string DivEnd = "</div>";
	private string HtmlTableEndCode = "</table>";
	
	#endregion
	
	#region Html Index File
	
	private string HtmlStart = "<!DOCTYPE html>" + 
		"<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
		"<head>" +
        "<title>Code Coverage Report</title>" +
        "<meta charset=\"utf-8\" />" +
        "<meta name=\"author\" content=\"Jose Miguel Malaca\" />" +
        "<meta name=\"description\" content=\"Code Coverage Report\" />" +
        "<meta name=\"keywords\" content=\"code,coverage,report,codecoveragereport,visual,studio,visualstudio\"/>" +
		"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
		"<link rel=\"stylesheet\" type=\"text/css\" href=\"./Classes/GlobalStyles.css\">" +
		"</head>" + 
		"<body>";
	private string DivIndexContentTable = "<div class=\"divIndexContentTable\">";
	private string HtmlIndexTableStart = "<table class=\"indexTable\">";
	private string ModulesHtmlIndexTableHeaders = "<tr>" +
		"<th>Name</th>" +
		"<th>Blocks Covered</th>" +
		"<th>Blocks Not Covered</th>" +
		"<th>Blocks Covered (%)</th>" +
		"<th>Blocks Not Covered (%)</th>" +
		"</tr>";
	
	#endregion
	
	#region Html Classes Files
	
	private string HtmlClassStart = "<!DOCTYPE html>" + 
		"<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
		"<head>" +
        "<title>Code Coverage Report</title>" +
        "<meta charset=\"utf-8\" />" +
        "<meta name=\"author\" content=\"Jose Miguel Malaca\" />" +
        "<meta name=\"description\" content=\"Code Coverage Report\" />" +
        "<meta name=\"keywords\" content=\"code,coverage,report,codecoveragereport,visual,studio,visualstudio\"/>" +
		"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
		"<link rel=\"stylesheet\" type=\"text/css\" href=\"GlobalStyles.css\">" +
		"</head>" + 
		"<body>";
	private string DivClassContentStart = "<div class=\"divClassContentTables\">";
	private string DivClassHeaderTableStart = "<div class=\"divClassHeaderTable\">";
	private string DivClassCodeTableStart = "<div class=\"divClassCodeTable\">";
	private string HtmlTableHeaderStartCode = "<table class=\"tableClassHeader\">";
	private string HtmlTableStartCode = "<table class=\"tableClassCode\">";
	
	#endregion
	
	#region Html Errors
	
	private string HtmlErrorsStart = "<!DOCTYPE html>" + 
		"<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
		"<head>" +
        "<title>Code Coverage Report</title>" +
        "<meta charset=\"utf-8\" />" +
        "<meta name=\"author\" content=\"Jose Miguel Malaca\" />" +
        "<meta name=\"description\" content=\"Code Coverage Report\" />" +
        "<meta name=\"keywords\" content=\"code,coverage,report,codecoveragereport,visual,studio,visualstudio\"/>" +
		"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
		"<link rel=\"stylesheet\" type=\"text/css\" href=\"./Classes/GlobalStyles.css\">" +
		"</head>" + 
		"<body>";
	private string DivErrorsContentTable = "<div class=\"divIndexContentTable\">";
	private string HtmlErrorsTableStart = "<table class=\"indexTable\">";
	private string ModulesHtmlErrorsTableHeaders = "<tr>" +
		"<th>Name</th>" +
		"<th>Error</th>" +
		"</tr>";
	private string ErrorsTableContent;
	
	#endregion
	
	#endregion

	#region constructor
	
	public HtmlReportWriter(CodeCoverageDataReader data, string path){
		this.CodeCoverageData = data;
		this.CodeCoverageHtmlPath = path;
	}
	
	#endregion
	
	#region Methods
	
	public void Generate()
	{
		this.PrintIndexHtmlFile();
		this.PrintClassesHtmlFiles();
		this.PrintCSSFile();
		
		this.ErrorsTableContent = string.Empty;
		this.PrintErrorsFile();
	}
	
	private void PrintIndexHtmlFile(){
		if(!Directory.Exists(this.CodeCoverageHtmlPath)){
			Directory.CreateDirectory(this.CodeCoverageHtmlPath);
		}
		this.HtmlTitle = "<p class=\"TitleName\">" + this.CodeCoverageData.fileName + " </p>";
		using (FileStream fs = new FileStream(CodeCoverageHtmlPath + "Index.html", FileMode.Create)) 
		{ 
			try{
				using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8)){ 
					w.WriteLine(this.HtmlStart);
					w.WriteLine(this.HtmlTitle);
					w.WriteLine(this.DivIndexContentTable);
					w.WriteLine(this.HtmlIndexTableStart);
					w.WriteLine(this.ModulesHtmlIndexTableHeaders);
					w.WriteLine(this.CodeCoverageData.ModulesHtmlTableContent);
					w.WriteLine(this.HtmlTableEndCode);
					w.WriteLine(this.DivEnd);
					w.WriteLine(this.HtmlEnd);
				}
			}catch(Exception e){
				ErrorsTableContent += "<tr>" +
					"<td class=\"tdFileName\" style=\"background:#A2A2A2; text-align: right;\"> Index.html </td>" +
					"<td class=\"tdError\" style=\"background:#A2A2A2; text-align: left;\">" + e.Message + "</td>" +
					"</tr>";
			}
		} 
	}
	
	private void PrintClassesHtmlFiles(){
		CodeCoverageData.ModulesData.ForEach(module => {
			module.NamespacesTableData.ForEach(nameSpace => {
				nameSpace.ClassesData.ForEach(classe => {
					this.HtmlTitle = "<p class=\"TitleName\">" + classe.ClassName + " </p>";
					var path = string.Empty;
					if (this.CodeCoverageHtmlPath[this.CodeCoverageHtmlPath.Length-1].Equals(@"\")){
						path = this.CodeCoverageHtmlPath + @"Classes\";
					}else{
						path = this.CodeCoverageHtmlPath + @"\Classes\";
					}
					if(!Directory.Exists(path)){
						Directory.CreateDirectory(path);
					}
					path = path + classe.ClassName + ".html";
					try
					{ 
						using (FileStream fs = new FileStream(path, FileMode.Create)) {
							using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8)) { 
								w.WriteLine(this.HtmlClassStart);
									w.WriteLine(this.HtmlTitle);
									w.WriteLine(this.DivClassContentStart);
										w.WriteLine(this.DivClassHeaderTableStart);
										w.WriteLine(this.HtmlTableHeaderStartCode);
										w.WriteLine(classe.HtmlClassTableHeader);
										w.WriteLine(this.HtmlTableEndCode);
									w.WriteLine(this.DivEnd);
									w.WriteLine(this.HtmlSpace);
									w.WriteLine(this.DivClassCodeTableStart);
										w.WriteLine(this.HtmlTableStartCode);
										classe.ClassFileLinesData.ForEach(line => w.WriteLine(line));
										w.WriteLine(this.HtmlTableEndCode);
										w.WriteLine(this.DivEnd);
									w.WriteLine(this.DivEnd);
								w.WriteLine(this.HtmlEnd);
							} 
						}
					}catch(Exception e){
						ErrorsTableContent += "<tr>" +
							"<td class=\"tdFileName\" style=\"background:#e0e2ef; text-align: left;\"> " + classe.ClassName + ".html </td>" +
							"<td class=\"tdError\" style=\"background:#e0e2ef; text-align: left;\">" + e.Message + "</td>" +
							"</tr>";
					}
				});
			});
		});
	}
	
	private void PrintCSSFile(){
		var path = string.Empty;
		if (this.CodeCoverageHtmlPath[this.CodeCoverageHtmlPath.Length-1].Equals(@"\")){
			path = this.CodeCoverageHtmlPath + @"Classes\";
		}else{
			path = this.CodeCoverageHtmlPath + @"\Classes\";
		}
		path = path + "GlobalStyles.css";
		try
		{ 
			using (FileStream fs = new FileStream(path, FileMode.Create)) {
				using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8)) { 
					w.WriteLine(".TitleName { color: #454545; font-size:30pt; text-align: center; margin:0; padding:4% 0% 4% 0%; } ");
					
					w.WriteLine(".divIndexContentTable { position: relative; left: 10%; width: 80%; margin: 0; padding: 0px; border: 0; font-size:14pt; }");
					w.WriteLine(".indexTable { width: 100%; margin: 0; padding: 0; border: 0; } ");
					
					w.WriteLine(".divClassContentTables { position: relative; left: 10%; width: 80%; margin: 0; padding: 0; border: 0; font-size:13pt; }");
					
					w.WriteLine(".divClassHeaderTable { width: 100%; margin: 0; padding: 0; border: 0; }");
					w.WriteLine(".tableClassHeader { border: 2px solid #4D4D4D; border-collapse: collapse; table-layout: fixed; width: 100%; }");
					w.WriteLine(".tdClassKeys { border: 2px solid #4D4D4D; width:25%; text-align:right; padding: 3px; }");
					w.WriteLine(".tdHeader { border: 2px solid #4D4D4D; padding: 3px; word-wrap:break-word; }");
					
					w.WriteLine(".divClassCodeTable { width: 100%; margin: 0; padding: 0; border: 0; }");
					w.WriteLine(".tableClassCode { border: 2px solid #4D4D4D; border-collapse: collapse; table-layout: fixed; width: 100%;}");
					w.WriteLine(".tdLineNumber { border: 2px solid #4D4D4D; width:5%; text-align:center; padding: 1px; }");
					w.WriteLine(".tdCode { border: 2px solid #4D4D4D; padding: 1px; word-wrap:break-word; }");
				} 
			}
		}catch(Exception e){
			Console.WriteLine("ERROR [ writting CSS ]");
		}
	}
	
	private void PrintErrorsFile(){
		if (this.ErrorsTableContent.Length > 0){
			if(!Directory.Exists(this.CodeCoverageHtmlPath)){
				Directory.CreateDirectory(this.CodeCoverageHtmlPath);
			}
			this.HtmlTitle = "<p class=\"TitleName\"> Errors </p>";
			using (FileStream fs = new FileStream(CodeCoverageHtmlPath + "errors.html", FileMode.Create)) 
			{ 
				try{
					using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8)){ 
						w.WriteLine(this.HtmlErrorsStart);
						w.WriteLine(this.HtmlTitle);
						w.WriteLine(this.DivErrorsContentTable);
						w.WriteLine(this.HtmlErrorsTableStart);
						w.WriteLine(this.ErrorsTableContent);
						w.WriteLine(this.DivErrorsContentTable);
						w.WriteLine(this.HtmlTableEndCode);
						w.WriteLine(this.DivEnd);
						w.WriteLine(this.HtmlEnd);
					}
				}catch(Exception e){
					Console.WriteLine("ERROR [ writting ERRORS file]: " + e.Message);
				}
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
			fileData.Path = filePath;
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
		this.GenerateHtmlTableClassHeader();
		this.GenerateHtmlTableRowCode();
		this.GenerateClassFileHtmlCode();
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
	
	//Html Class Table Header
	public string HtmlClassTableHeader
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
	
	//Html Class File Data
	public List<string> ClassFileLinesData
	{
		get;
		set;
	}
	
	#endregion
	
	#region Methods
	
	private void GetClassData(XElement classe, List<FileData> filesData){
		this.ClassKeyName = classe.Element("ClassKeyName").Value;
		
		this.ClassName = GetValidPathName(classe.Element("ClassName").Value);
		
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
	
	private string GetValidPathName(string value){
		var pattern = @"[^\w]";
		var rgx = new Regex(pattern);
		return rgx.Replace(value, @"_");
	}
	
	private void GenerateHtmlTableClassHeader(){
		var fileData = GetClassFileData();
		this.HtmlClassTableHeader = "<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#A2A2A2; text-align: right;\"> Class Key Name </td>" +
			"<td class=\"tdHeader\" style=\"background:#A2A2A2; text-align: left;\">" + this.ClassKeyName + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#CCCCCC; text-align: right;\"> Class Name </td>" +
			"<td class=\"tdHeader\" style=\"background:#CCCCCC; text-align: left;\">" + this.ClassName + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#A2A2A2; text-align: right;\"> Lines Covered </td>" +
			"<td class=\"tdHeader\" style=\"background:#A2A2A2; text-align: left;\">" + this.LinesCovered + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#CCCCCC; text-align: right;\"> Lines Not Covered </td>" +
			"<td class=\"tdHeader\" style=\"background:#CCCCCC; text-align: left;\">" + this.LinesNotCovered + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#A2A2A2; text-align: right;\"> Lines Partially Covered </td>" +
			"<td class=\"tdHeader\" style=\"background:#A2A2A2; text-align: left;\">" + this.LinesPartiallyCovered + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#CCCCCC; text-align: right;\"> Blocks Covered </td>" +
			"<td class=\"tdHeader\" style=\"background:#CCCCCC; text-align: left;\">" + this.BlocksCovered + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#A2A2A2; text-align: right;\"> Blocks Not Covered </td>" +
			"<td class=\"tdHeader\" style=\"background:#A2A2A2; text-align: left;\">" + this.BlocksNotCovered + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#CCCCCC; text-align: right;\"> Blocks Covered (%) </td>" +
			"<td class=\"tdHeader\" style=\"background:#CCCCCC; text-align: left;\">" + this.BlocksCoveredPercentage + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#A2A2A2; text-align: right;\"> Blocks Not Covered (%) </td>" +
			"<td class=\"tdHeader\" style=\"background:#A2A2A2; text-align: left;\">" + this.BlocksNotCoveredPercentage + "</td>" +
			"</tr>" +
			"<tr>" +
			"<td class=\"tdClassKeys\" style=\"background:#CCCCCC; text-align: right;\"> File Path </td>" +
			"<td class=\"tdHeader\" style=\"background:#CCCCCC; text-align: left;\">" + fileData.Path + "</td>" +
			"</tr>";
	}
	
	private void GenerateHtmlTableRowCode(){
		if (this.AddClassesRowsData){
			this.HtmlTableRowCode = "<tr>" +
				"<td style=\"background:#99C2D6; text-align: left;\">" + this.Spaces + this.Spaces + this.HtmlBlackDot + "<a href=\"./Classes/" + this.ClassName + ".html"  + "\">Class: " + this.ClassName + "</a></td>" +
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
	
	private void GenerateClassFileHtmlCode(){
		var fileData = GetClassFileData();
		this.ClassFileLinesData = GetLinesFromFile(fileData.Path);
		this.MethodsData.ForEach(method => {
			method.LinesData.ForEach(line => {
				var number = Int32.Parse(line.LnStart);
				var lineStr = this.ClassFileLinesData[number-1];
				var coverage = Int32.Parse(line.Coverage);
				if (coverage == 0){
					this.ClassFileLinesData[number-1] = this.ClassFileLinesData[number-1].Replace("99C2D6","66E066");
					this.ClassFileLinesData[number-1] = this.ClassFileLinesData[number-1].Replace("B8D4E2","85E685");
				}else{
					this.ClassFileLinesData[number-1] = this.ClassFileLinesData[number-1].Replace("99C2D6","FF4D4D");
					this.ClassFileLinesData[number-1] = this.ClassFileLinesData[number-1].Replace("B8D4E2","FF8282");
				}
			});
		});
	}
	
	private FileData GetClassFileData(){
		if (this.MethodsData != null){
			var someMethodsData = this.MethodsData[0];
			if (someMethodsData.LinesData != null){
				var someLineData = someMethodsData.LinesData[0];
				return someLineData.SourceFileData;
			}
		}
		return null;
	}
	
	private List<string> GetLinesFromFile(string filePath){
		var lines = new List<string>();
		var file = new System.IO.StreamReader(filePath);
		var line = string.Empty;
		var count = 0;
		while((line = file.ReadLine()) != null)
		{
			line = new Regex(@"\s").Replace(line, "&nbsp;");
			line = line.Trim();
			lines.Add("<tr>" +
				"<td class=\"tdLineNumber\" style=\"background:#99C2D6; text-align: center;\"> " + (count+1) + " </td>" +
    			"<td class=\"tdCode\" style=\"background:#B8D4E2;\"> " + line + " </td>" +
				"</tr>");
			count++;
		}
		file.Close();
		return lines;
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
