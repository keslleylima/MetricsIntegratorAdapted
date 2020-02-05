using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetricDataIntegrator
{

    public class SourceTestMetrics
    {
        public int countInput = 0;
        public int countLineCode = 0;
        public int countLineCodeDecl = 0;
        public int countLineCodeExe = 0;
        public int countOutput = 0;
        public int countPath = 0;
        public int countPathLog = 0;
        public int countStmt = 0;
        public int countStmtDecl = 0;
        public int countStmtExe = 0;
        public int cyclomatic = 0;
        public int cyclomaticModified = 0;
        public int cyclomaticStrict = 0;
        public int essential = 0;
        public int knots = 0;
        public int maxEssentialKnots = 0;
        public int maxNesting = 0;
        public int minEssentialKnots = 0;
    }
    public class SourceCodeMetrics
    {
        public int countInput;
        public int countLineCode;
        public int countLineCodeDecl;
        public int countLineCodeExe;
        public int countOutput;
        public int countPath;
        public int countPathLog;
        public int countStmt;
        public int countStmtDecl;
        public int countStmtExe;
        public int cyclomatic;
        public int cyclomaticModified;
        public int cyclomaticStrict;
        public int essential;
        public int knots;
        public int maxEssentialKnots;
        public int maxNesting;
        public int minEssentialKnots;
    }

    public class TestPathMetrics
    {
        public string id;
        public string testPath;
        public int pathLength;
        public int hasLoop;
        public int countLoop;
        public int countnewReqNcCovered;
        public int countReqNcCovered;
        public double nodeCoverage;
        public int countnewReqPpcCovered;
        public int countReqPcCovered;
        public double primePathCoverage;
    }
    public class TestCaseMetrics
    {
        public string id;
        public double avgPathLength;
        public int hasLoop;
        public double avgCountLoop;
        public int countReqEcCovered;
        public double edgeCoverage;
        public int countReqPcCovered;
        public double primePathCoverage;
    }
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = @"PROJECT_PATH"; //For example, D:\GitHub\TCmetricsGeneratorAdapted\Projects\Mp3agic
            string[] csvFilesPath = GetFiles(projectPath);
            string smPath = string.Empty;
            string mapPath = string.Empty;
            string[] sourceMetricsFile;
            string[] metricsMappingFile;
            string testPathsPath = string.Empty;
            string testCasePath = string.Empty;
            string[] testPathMetricsFile;
            string[] testCaseMetricsFile;

            Dictionary<string, SourceCodeMetrics> dictSourceCode = new Dictionary<string, SourceCodeMetrics>();
            Dictionary<string, SourceTestMetrics> dictSourceTest = new Dictionary<string, SourceTestMetrics>();
            Dictionary<string, string[]> mapping = new Dictionary<string, string[]>();

            List<TestPathMetrics> listTestPath = new List<TestPathMetrics>();
            List<TestCaseMetrics> listTestCase = new List<TestCaseMetrics>();

            foreach (string csvPath in csvFilesPath)
            {
                if (csvPath.Contains("SCM_"))
                    smPath = csvPath;

                if (csvPath.Contains("MAP_"))
                    mapPath = csvPath;

                if (csvPath.Contains("TestPath_"))
                    testPathsPath = csvPath;

                if (csvPath.Contains("TestCase_"))
                    testCasePath = csvPath;
            }

            sourceMetricsFile = OpenFile(smPath);
            metricsMappingFile = OpenFile(mapPath);
            testPathMetricsFile = OpenFile(testPathsPath);
            testCaseMetricsFile = OpenFile(testCasePath);


            foreach (string line in metricsMappingFile)
            {
                string[] column;
                column = line.Split(";");
                mapping.Add(column[0], column[1..column.Length]);
            }

            foreach (string line in testPathMetricsFile.Skip(1).ToArray())
            {
                TestPathMetrics testPath = new TestPathMetrics();
                string[] column;
                column = line.Split(";");
                listTestPath.Add(SetTestPathMetrics(testPath, column));
            }

            foreach (string line in testCaseMetricsFile.Skip(1).ToArray())
            {
                TestCaseMetrics testCase = new TestCaseMetrics();
                string[] column;
                column = line.Split(";");
                listTestCase.Add(SetTestCaseMetrics(testCase, column));
            }

            foreach (string line in sourceMetricsFile.Skip(1).ToArray())
            {
                string[] column;
                column = line.Split(";");
                if (mapping.ContainsKey(column[1]))
                {
                    SourceCodeMetrics metricsSourceCode = new SourceCodeMetrics();
                    SetSourceCodeMetrics(metricsSourceCode, column);
                    dictSourceCode.Add(column[1], metricsSourceCode);


                }
                else
                {
                    foreach (KeyValuePair<string, string[]> kvp in mapping)
                    {
                        string[] keysTest = kvp.Value;
                        foreach (string key in keysTest)
                        {
                            if (key == column[1])
                            {
                                SourceTestMetrics metricsSourceTest = new SourceTestMetrics();
                                SetSourceTestMetrics(metricsSourceTest, column);
                                dictSourceTest.Add(column[1], metricsSourceTest);

                            }
                        }

                    }

                }
            }
            string TestPathFilePath = @"D:\GitHub\MetricsIntegratorAdapted\TP_dataset_resulting_" + projectPath.Substring(projectPath.LastIndexOf(@"\") + 1) + ".csv";
            string TestCaseFilePath = @"D:\GitHub\MetricsIntegratorAdapted\TC_dataset_resulting_" + projectPath.Substring(projectPath.LastIndexOf(@"\") + 1) + ".csv";

            CsvIntegratorTestPath(TestPathFilePath, mapping, dictSourceCode, dictSourceTest, listTestPath);
            CsvIntegratorTestCase(TestCaseFilePath, mapping, dictSourceCode, dictSourceTest, listTestCase);
        }
        private static string[] OpenFile(string path)
        {
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (FileNotFoundException execption)
            {
                throw execption;
            }
            return lines;
        }
        public static string[] GetFiles(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath, "*.csv", System.IO.SearchOption.AllDirectories);

            return files;
        }

        public static void SetSourceCodeMetrics(SourceCodeMetrics msc, string[] row)
        {
            msc.countInput = Int32.Parse(row[2]);
            msc.countLineCode = Int32.Parse(row[3]);
            msc.countLineCodeDecl = Int32.Parse(row[4]);
            msc.countLineCodeExe = Int32.Parse(row[5]);
            msc.countOutput = Int32.Parse(row[6]);
            msc.countPath = Int32.Parse(row[7]);
            msc.countPathLog = Int32.Parse(row[8]);
            msc.countStmt = Int32.Parse(row[9]);
            msc.countStmtDecl = Int32.Parse(row[10]);
            msc.countStmtExe = Int32.Parse(row[11]);
            msc.cyclomatic = Int32.Parse(row[12]);
            msc.cyclomaticModified = Int32.Parse(row[13]);
            msc.cyclomaticStrict = Int32.Parse(row[14]);
            msc.essential = Int32.Parse(row[15]);
            msc.knots = Int32.Parse(row[16]);
            msc.maxEssentialKnots = Int32.Parse(row[17]);
            msc.maxNesting = Int32.Parse(row[18]);
            msc.minEssentialKnots = Int32.Parse(row[19]);
        }
        public static void SetSourceTestMetrics(SourceTestMetrics mst, string[] row)
        {
            mst.countInput = Int32.Parse(row[2]);
            mst.countLineCode = Int32.Parse(row[3]);
            mst.countLineCodeDecl = Int32.Parse(row[4]);
            mst.countLineCodeExe = Int32.Parse(row[5]);
            mst.countOutput = Int32.Parse(row[6]);
            mst.countPath = Int32.Parse(row[7]);
            mst.countPathLog = Int32.Parse(row[8]);
            mst.countStmt = Int32.Parse(row[9]);
            mst.countStmtDecl = Int32.Parse(row[10]);
            mst.countStmtExe = Int32.Parse(row[11]);
            mst.cyclomatic = Int32.Parse(row[12]);
            mst.cyclomaticModified = Int32.Parse(row[13]);
            mst.cyclomaticStrict = Int32.Parse(row[14]);
            mst.essential = Int32.Parse(row[15]);
            mst.knots = Int32.Parse(row[16]);
            mst.maxEssentialKnots = Int32.Parse(row[17]);
            mst.maxNesting = Int32.Parse(row[18]);
            mst.minEssentialKnots = Int32.Parse(row[19]);
        }

        public static TestPathMetrics SetTestPathMetrics(TestPathMetrics tpm, string[] row)
        {
            tpm.id = row[0];
            tpm.testPath = row[1];
            tpm.pathLength = Int32.Parse(row[2]);
            tpm.hasLoop = Int32.Parse(row[3]);
            tpm.countLoop = Int32.Parse(row[4]);
            tpm.countnewReqNcCovered = Int32.Parse(row[5]);
            tpm.countReqNcCovered = Int32.Parse(row[6]);
            tpm.nodeCoverage = Double.Parse(row[7]);
            tpm.countnewReqPpcCovered = Int32.Parse(row[8]);
            tpm.countReqPcCovered = Int32.Parse(row[9]);
            tpm.primePathCoverage = Double.Parse(row[10]);

            return tpm;
        }
        public static TestCaseMetrics SetTestCaseMetrics(TestCaseMetrics tcm, string[] row)
        {
            tcm.id = row[0];
            tcm.avgPathLength = Double.Parse(row[1]);
            tcm.hasLoop = Int32.Parse(row[2]);
            tcm.avgCountLoop = Double.Parse(row[3]);
            tcm.countReqEcCovered = Int32.Parse(row[4]);
            tcm.edgeCoverage = Double.Parse(row[5]);
            tcm.countReqPcCovered = Int32.Parse(row[6]);
            tcm.primePathCoverage = Double.Parse(row[7]);

            return tcm;
        }
        public static void CsvIntegratorTestPath(string filePath, Dictionary<string, string[]> mapping, Dictionary<string, SourceCodeMetrics> dictSourceCode,
            Dictionary<string, SourceTestMetrics> dictSourceTest, List<TestPathMetrics> listTestPath)
        {

            if (File.Exists(filePath))
                File.Delete(filePath);

            string delimiter = ";";
            StringBuilder sb = new StringBuilder();

            if (!File.Exists(filePath))
                sb.Append("ID;countInput;countLineCode;countLineCodeDecl;countLineCodeExe;" +
                    "countOutput;countPath;countPathLog;countStmt;countStmtDec;" +
                    "countStmtExe;cyclomatic;cyclomaticModified;cyclomaticStrict;essential;knots;" +
                    "maxEssentialKnots;maxNesting;minEssentialKnots;ID;countInput;countLineCode;countLineCodeDecl;" +
                    "countLineCodeExe;countOutput;countPath;countPathLog;countStmt;countStmtDec;" +
                    "countStmtExe;cyclomatic;cyclomaticModified;cyclomaticStrict;essential;knots;maxEssentialKnots;" +
                    "maxNesting;minEssentialKnots;id;testPath;pathLength;hasLoop;countLoop;countnewReqEcCovered;" +
                    "countReqEcCovered;EdgeCoverage;countnewReqPpcCovered;countReqPcCovered;primePathCoverage" + "\n");
            foreach (KeyValuePair<string, string[]> kvp in mapping)
            {
                string KeyCode = kvp.Key;
                string[] keysTest = kvp.Value;

                dictSourceCode.TryGetValue(KeyCode, out SourceCodeMetrics metricsSourceCode);

                foreach (string keyTest in keysTest)
                {

                    dictSourceTest.TryGetValue(keyTest, out SourceTestMetrics metricsSourceTest);

                    foreach (TestPathMetrics tpMetrics in listTestPath)
                    {
                        if (tpMetrics.id == keyTest)
                        {


                            sb.Append(KeyCode + delimiter + metricsSourceCode.countInput + delimiter + metricsSourceCode.countLineCode
                            + delimiter + metricsSourceCode.countLineCodeDecl + delimiter + metricsSourceCode.countLineCodeExe
                            + delimiter + metricsSourceCode.countOutput + delimiter + metricsSourceCode.countPath
                            + delimiter + metricsSourceCode.countPathLog + delimiter + metricsSourceCode.countStmt + delimiter + metricsSourceCode.countStmtDecl
                            + delimiter + metricsSourceCode.countStmtExe + delimiter + metricsSourceCode.cyclomatic
                            + delimiter + metricsSourceCode.cyclomaticModified + delimiter + metricsSourceCode.cyclomaticStrict
                            + delimiter + metricsSourceCode.essential + delimiter + metricsSourceCode.knots
                            + delimiter + metricsSourceCode.maxEssentialKnots + delimiter + metricsSourceCode.maxNesting
                            + delimiter + metricsSourceCode.minEssentialKnots + delimiter);

                            sb.Append(keyTest + delimiter + metricsSourceTest.countInput + delimiter + metricsSourceTest.countLineCode
                            + delimiter + metricsSourceTest.countLineCodeDecl + delimiter + metricsSourceTest.countLineCodeExe
                            + delimiter + metricsSourceTest.countOutput + delimiter + metricsSourceTest.countPath
                            + delimiter + metricsSourceTest.countPathLog + delimiter + metricsSourceTest.countStmt + delimiter + metricsSourceTest.countStmtDecl
                            + delimiter + metricsSourceTest.countStmtExe + delimiter + metricsSourceTest.cyclomatic
                            + delimiter + metricsSourceTest.cyclomaticModified + delimiter + metricsSourceTest.cyclomaticStrict
                            + delimiter + metricsSourceTest.essential + delimiter + metricsSourceTest.knots
                            + delimiter + metricsSourceTest.maxEssentialKnots + delimiter + metricsSourceTest.maxNesting
                            + delimiter + metricsSourceTest.minEssentialKnots + delimiter);

                            sb.Append(tpMetrics.id + delimiter + tpMetrics.testPath + delimiter
                                + tpMetrics.pathLength + delimiter + tpMetrics.hasLoop + delimiter
                                + tpMetrics.countLoop + delimiter + tpMetrics.countnewReqNcCovered + delimiter
                                + tpMetrics.countReqNcCovered + delimiter + tpMetrics.nodeCoverage + delimiter
                                + tpMetrics.countnewReqPpcCovered + delimiter + tpMetrics.countReqPcCovered + delimiter
                                + tpMetrics.primePathCoverage + delimiter + "\n");

                        }
                    }
                }
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        public static void CsvIntegratorTestCase(string filePath, Dictionary<string, string[]> mapping, Dictionary<string, SourceCodeMetrics> dictSourceCode,
            Dictionary<string, SourceTestMetrics> dictSourceTest, List<TestCaseMetrics> listTestCase)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            string delimiter = ";";
            StringBuilder sb = new StringBuilder();

            if (!File.Exists(filePath))
                sb.Append("ID;countInput;countLineCode;countLineCodeDecl;countLineCodeExe;" +
                    "countOutput;countPath;countPathLog;countStmt;countStmtDec;" +
                    "countStmtExe;cyclomatic;cyclomaticModified;cyclomaticStrict;essential;knots;" +
                    "maxEssentialKnots;maxNesting;minEssentialKnots;ID;countInput;countLineCode;countLineCodeDecl;" +
                    "countLineCodeExe;countOutput;countPath;countPathLog;countStmt;countStmtDec;" +
                    "countStmtExe;cyclomatic;cyclomaticModified;cyclomaticStrict;essential;knots;maxEssentialKnots;" +
                    "maxNesting;minEssentialKnots;Id;AvgPathLength;HasLoop;AvgCountLoop;CountReqEcCovered;EdgeCoverage;CountReqPcCovered;PrimePathCoverage\n");

            foreach (KeyValuePair<string, string[]> kvp in mapping)
            {
                string keyCode = kvp.Key;
                string[] keysTest = kvp.Value;

                dictSourceCode.TryGetValue(keyCode, out SourceCodeMetrics metricsSourceCode);
                SourceTestMetrics acumulation = new SourceTestMetrics();
                TestCaseMetrics acumlationSUM = new TestCaseMetrics();

                foreach (string keyTest in keysTest)
                {

                    dictSourceTest.TryGetValue(keyTest, out SourceTestMetrics metricsSourceTest);

                    if (metricsSourceTest != null)
                    {
                        acumulation.countInput += metricsSourceTest.countInput;
                        acumulation.countLineCode += metricsSourceTest.countLineCode;
                        acumulation.countLineCodeDecl += metricsSourceTest.countLineCodeDecl;
                        acumulation.countLineCodeExe += metricsSourceTest.countLineCodeExe;
                        acumulation.countOutput += metricsSourceTest.countOutput;
                        acumulation.countPath += metricsSourceTest.countPath;
                        acumulation.countPathLog += metricsSourceTest.countPathLog;
                        acumulation.countStmt += metricsSourceTest.countStmt;
                        acumulation.countStmtDecl += metricsSourceTest.countStmtDecl;
                        acumulation.countStmtExe += metricsSourceTest.countStmtExe;
                        acumulation.cyclomatic += metricsSourceTest.cyclomatic;
                        acumulation.cyclomaticModified += metricsSourceTest.cyclomaticModified;
                        acumulation.cyclomaticStrict += metricsSourceTest.cyclomaticStrict;
                        acumulation.essential += metricsSourceTest.essential;
                        acumulation.knots += metricsSourceTest.knots;
                    }

                    foreach (TestCaseMetrics tcMetrics in listTestCase)
                    {
                        if (tcMetrics.id == keyTest)
                        {
                            acumlationSUM.id = tcMetrics.id;
                            acumlationSUM.hasLoop += tcMetrics.hasLoop;
                            acumlationSUM.avgCountLoop += tcMetrics.avgCountLoop;
                            acumlationSUM.avgPathLength += tcMetrics.avgPathLength;
                            acumlationSUM.countReqEcCovered += tcMetrics.countReqEcCovered;
                            acumlationSUM.countReqPcCovered += tcMetrics.countReqPcCovered;
                            acumlationSUM.edgeCoverage += tcMetrics.edgeCoverage;
                            acumlationSUM.primePathCoverage += tcMetrics.primePathCoverage;
                        }
                    }
                }
                sb.Append(keyCode + delimiter + metricsSourceCode.countInput + delimiter + metricsSourceCode.countLineCode
                            + delimiter + metricsSourceCode.countLineCodeDecl + delimiter + metricsSourceCode.countLineCodeExe
                            + delimiter + metricsSourceCode.countOutput + delimiter + metricsSourceCode.countPath
                            + delimiter + metricsSourceCode.countPathLog + delimiter + metricsSourceCode.countStmt + delimiter + metricsSourceCode.countStmtDecl
                            + delimiter + metricsSourceCode.countStmtExe + delimiter + metricsSourceCode.cyclomatic
                            + delimiter + metricsSourceCode.cyclomaticModified + delimiter + metricsSourceCode.cyclomaticStrict
                            + delimiter + metricsSourceCode.essential + delimiter + metricsSourceCode.knots
                            + delimiter + metricsSourceCode.maxEssentialKnots + delimiter + metricsSourceCode.maxNesting
                            + delimiter + metricsSourceCode.minEssentialKnots + delimiter);


                sb.Append(acumlationSUM.id + delimiter + acumulation.countInput + delimiter + acumulation.countLineCode
                + delimiter + acumulation.countLineCodeDecl + delimiter + acumulation.countLineCodeExe
                + delimiter + acumulation.countOutput + delimiter + acumulation.countPath
                + delimiter + acumulation.countPathLog + delimiter + acumulation.countStmt + delimiter + acumulation.countStmtDecl
                + delimiter + acumulation.countStmtExe + delimiter + acumulation.cyclomatic
                + delimiter + acumulation.cyclomaticModified + delimiter + acumulation.cyclomaticStrict
                + delimiter + acumulation.essential + delimiter + acumulation.knots
                + delimiter + acumulation.maxEssentialKnots + delimiter + acumulation.maxNesting + delimiter + acumulation.minEssentialKnots + delimiter);

                int hasloop = 0;
                if (acumlationSUM.hasLoop == 0)
                {
                    hasloop = 0;
                }
                else if (acumlationSUM.hasLoop >= 1)
                {
                    hasloop = 1;
                }

                sb.Append(acumlationSUM.id + delimiter + acumlationSUM.avgPathLength + delimiter
                                + hasloop + delimiter + acumlationSUM.avgCountLoop + delimiter
                                + acumlationSUM.countReqEcCovered + delimiter + acumlationSUM.edgeCoverage + delimiter
                                + acumlationSUM.countReqPcCovered + delimiter + acumlationSUM.primePathCoverage + delimiter + "\n");
            }
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}