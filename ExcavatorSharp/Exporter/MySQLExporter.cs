// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Exporter.MySQLExporter
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcavatorSharp.Common;
using ExcavatorSharp.Excavator;
using ExcavatorSharp.ExtensionMethods;
using ExcavatorSharp.Grabber;
using ExcavatorSharp.Interfaces;
using ExcavatorSharp.Objects;

namespace ExcavatorSharp.Exporter
{
    /// <summary>
    /// Exporting for saving data into .mysql file
    /// </summary>
    internal class MySQLExporter : IDataContextualExportable
    {
    	/// <summary>
    	/// Blocks limit for `Insert many` command
    	/// </summary>
    	public const int InsertDataRowBlocksLimit = 20;

    	/// <summary>
    	/// Actual blocks count for `Insert many` command
    	/// </summary>
    	private int ActualDataRowBlocksCount = 0;

    	/// <summary>
    	/// Actual blocks data writer
    	/// </summary>
    	private StringBuilder ActualDataRowBlocksWriter = new StringBuilder();

    	/// <summary>
    	/// List of MySQL column pointers that associates data with table column names
    	/// </summary>
    	private List<MySQLColumnPointer> DataColumnsPointers = new List<MySQLColumnPointer>();

    	/// <summary>
    	/// Name of exporting table
    	/// </summary>
    	private string MySQLTableName { get; set; }

    	/// <summary>
    	/// Items sequences separator
    	/// </summary>
    	private string ItemsSequencesSeparator { get; set; }

    	/// <summary>
    	/// Link to saving file
    	/// </summary>
    	private string FileSavingPath { get; set; }

    	/// <summary>
    	/// Type of exporting data
    	/// </summary>
    	private DataExportingType DataExportingType { get; set; }

    	/// <summary>
    	/// Link to the task logger
    	/// </summary>
    	private DataExcavatorTasksLogger TaskLoggerLink { get; set; }

    	/// <summary>
    	/// Adds data to exporting file
    	/// </summary>
    	/// <param name="ExportingData"></param>
    	public void AddDataIntoExportingFile(List<GrabbedDataGroup> ExportingData)
    	{
    		for (int i = 0; i < ExportingData.Count; i++)
    		{
    			int patternHash = ExportingData[i].PatternHash;
    			List<MySQLColumnPointer> list = new List<MySQLColumnPointer>();
    			for (int j = 0; j < DataColumnsPointers.Count; j++)
    			{
    				if (DataColumnsPointers[j].PatternHash == patternHash)
    				{
    					list.Add(DataColumnsPointers[j]);
    				}
    			}
    			for (int k = 0; k < ExportingData[i].GrabbingResults.Count; k++)
    			{
    				if (ActualDataRowBlocksCount == 0)
    				{
    					ActualDataRowBlocksWriter.Append("INSERT INTO `" + MySQLTableName + "` (DataParsedDateTime, GrabbedPageURL, PatternHash, PatternName, IsEmptyResultset, ");
    					for (int l = 0; l < list.Count; l++)
    					{
    						ActualDataRowBlocksWriter.Append(list[l].ColumnName);
    						if (l + 1 < list.Count)
    						{
    							ActualDataRowBlocksWriter.Append(", ");
    						}
    					}
    					ActualDataRowBlocksWriter.Append(") VALUES ");
    				}
    				ActualDataRowBlocksWriter.Append('(');
    				ActualDataRowBlocksWriter.Append('\'').Append(ExportingData[i].PageGrabbedDateTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("',");
    				ActualDataRowBlocksWriter.Append('\'').Append(ProcessDBValue(ExportingData[i].GrabbedPageUrl)).Append("',");
    				ActualDataRowBlocksWriter.Append('\'').Append(ExportingData[i].PatternHash).Append("',");
    				ActualDataRowBlocksWriter.Append('\'').Append(ProcessDBValue(ExportingData[i].PatternName)).Append("',");
    				ActualDataRowBlocksWriter.Append(ExportingData[i].IsEmptyResultSet ? "1" : "0").Append(",");
    				for (int m = 0; m < list.Count; m++)
    				{
    					bool flag = false;
    					for (int n = 0; n < ExportingData[i].GrabbingResults[k].Count; n++)
    					{
    						if (list[m].PointerType == MySQLColumnPointerType.ContentData && ExportingData[i].GrabbingResults[k][n].DataGrabbingPatternItemElementName == list[m].PatternItemElementName)
    						{
    							ActualDataRowBlocksWriter.Append('\'');
    							for (int num = 0; num < ExportingData[i].GrabbingResults[k][n].GrabbedItemsData.Length; num++)
    							{
    								if (DataExportingType == DataExportingType.InnerText)
    								{
    									ActualDataRowBlocksWriter.Append(ProcessDBValue(ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num].ElementInnerText));
    								}
    								else if (DataExportingType == DataExportingType.OuterHTML)
    								{
    									ActualDataRowBlocksWriter.Append(ProcessDBValue(ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num].ElementOuterHtml));
    								}
    								if (num + 1 < ExportingData[i].GrabbingResults[k][n].GrabbedItemsData.Length)
    								{
    									ActualDataRowBlocksWriter.Append(ItemsSequencesSeparator);
    								}
    							}
    							ActualDataRowBlocksWriter.Append('\'');
    							flag = true;
    							break;
    						}
    						if (list[m].PointerType != MySQLColumnPointerType.AttributeData && DataColumnsPointers[m].PointerType != MySQLColumnPointerType.AttributeFileName)
    						{
    							continue;
    						}
    						List<string> list2 = new List<string>();
    						bool flag2 = false;
    						for (int num2 = 0; num2 < ExportingData[i].GrabbingResults[k][n].GrabbedItemsData.Length; num2++)
    						{
    							for (int num3 = 0; num3 < ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num2].ElementAttributes.Length; num3++)
    							{
    								string text = string.Empty;
    								if (DataColumnsPointers[m].PointerType == MySQLColumnPointerType.AttributeData)
    								{
    									text = $"Attribute-{patternHash}-{ExportingData[i].GrabbingResults[k][n].DataGrabbingPatternItemElementName}-{ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num2].ElementAttributes[num3].AttributeName}-OrigData";
    								}
    								else if (DataColumnsPointers[m].PointerType == MySQLColumnPointerType.AttributeFileName)
    								{
    									text = $"Attribute-{patternHash}-{ExportingData[i].GrabbingResults[k][n].DataGrabbingPatternItemElementName}-{ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num2].ElementAttributes[num3].AttributeName}-SavedFiles";
    								}
    								if (text == list[m].PatternItemElementName)
    								{
    									if (DataColumnsPointers[m].PointerType == MySQLColumnPointerType.AttributeData)
    									{
    										list2.Add(ProcessDBValue(ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num2].ElementAttributes[num3].AttributeValue));
    									}
    									else if (DataColumnsPointers[m].PointerType == MySQLColumnPointerType.AttributeFileName)
    									{
    										list2.Add(ProcessDBValue(ExportingData[i].GrabbingResults[k][n].GrabbedItemsData[num2].ElementAttributes[num3].AttributeSavedFileName));
    									}
    									flag2 = true;
    								}
    							}
    						}
    						if (flag2)
    						{
    							string value = string.Join(ItemsSequencesSeparator, list2);
    							ActualDataRowBlocksWriter.Append('\'').Append(value).Append('\'');
    							flag = true;
    							break;
    						}
    					}
    					if (!flag)
    					{
    						ActualDataRowBlocksWriter.Append("''");
    					}
    					if (m + 1 < list.Count)
    					{
    						ActualDataRowBlocksWriter.Append(',');
    					}
    				}
    				ActualDataRowBlocksWriter.Append(')');
    				ActualDataRowBlocksCount++;
    				if (k + 1 < ExportingData[i].GrabbingResults.Count && ActualDataRowBlocksCount <= 20)
    				{
    					ActualDataRowBlocksWriter.Append(',');
    					continue;
    				}
    				ActualDataRowBlocksWriter.Append(';').Append("\r\n");
    				FlushInsertingDataToHDD();
    			}
    		}
    	}

    	/// <summary>
    	/// Flushed inserting data buffer to HDD
    	/// </summary>
    	private void FlushInsertingDataToHDD()
    	{
    		ActualDataRowBlocksCount = 0;
    		string contents = ActualDataRowBlocksWriter.ToString();
    		ActualDataRowBlocksWriter.Clear();
    		File.AppendAllText(FileSavingPath, contents);
    	}

    	/// <summary>
    	/// Close file and finish export
    	/// </summary>
    	public void FinishExport()
    	{
    		FlushInsertingDataToHDD();
    	}

    	/// <summary>
    	/// Initializes exporter
    	/// </summary>
    	/// <param name="FileSavingPath">Path to exporting file</param>
    	/// <param name="FoundedDataExportingType">Type of exporting data</param>
    	/// <param name="ItemsSequencesSeparator">Separator for separating data sequences, if some data includes many items</param>
    	/// <param name="ProjectPatternsList">All project patterns list from first grabbing</param>
    	public void InitializeExpoter(string FileSavingPath, DataExportingType FoundedDataExportingType, string ItemsSequencesSeparator, List<DataGrabbingPattern> ProjectPatternsList, DataExcavatorTasksLogger TaskLoggerLink)
    	{
    		this.FileSavingPath = FileSavingPath;
    		DataExportingType = DataExportingType;
    		this.ItemsSequencesSeparator = ProcessDBValue(ItemsSequencesSeparator);
    		this.TaskLoggerLink = TaskLoggerLink;
    		int num = 0;
    		for (int i = 0; i < ProjectPatternsList.Count; i++)
    		{
    			num += ProjectPatternsList[i].GetHashCode();
    		}
    		if (num < 0)
    		{
    			num *= -1;
    		}
    		MySQLTableName = $"ExportedData_{num.ToString()}";
    		string text = "\r\nCREATE TABLE `" + MySQLTableName + "` (\r\n    `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT,\r\n    `DataParsedDateTime` DATETIME NOT NULL,\r\n    `GrabbedPageURL` TEXT NOT NULL,\r\n    `PatternHash` VARCHAR(255) NOT NULL,\r\n    `PatternName` VARCHAR(255) NOT NULL,\r\n    `IsEmptyResultset` TINYINT(1) NOT NULL,";
    		text += "\r\n";
    		for (int j = 0; j < ProjectPatternsList.Count; j++)
    		{
    			for (int k = 0; k < ProjectPatternsList[j].GrabbingItemsPatterns.Count; k++)
    			{
    				string elementName = ProjectPatternsList[j].GrabbingItemsPatterns[k].ElementName;
    				string text2 = $"Data_{j}_{k}_{PrepareColumnName(elementName)}";
    				MySQLColumnPointer item = new MySQLColumnPointer(ProjectPatternsList[j].GetHashCode(), elementName, text2, MySQLColumnPointerType.ContentData);
    				text += $"\t`{text2}` TEXT NULL,\r\n";
    				DataColumnsPointers.Add(item);
    				if (!ProjectPatternsList[j].GrabbingItemsPatterns[k].ParseBinaryAttributes || ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes.Length == 0)
    				{
    					continue;
    				}
    				for (int l = 0; l < ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes.Length; l++)
    				{
    					string text3 = string.Format("{0}_attrOrigData", text2, PrepareColumnName(ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes[l].AttributeName));
    					text += $"\t`{text3}` TEXT NULL,\r\n";
    					string patternItemElementName = $"Attribute-{ProjectPatternsList[j].GetHashCode()}-{ProjectPatternsList[j].GrabbingItemsPatterns[k].ElementName}-{ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes[l].AttributeName}-OrigData";
    					DataColumnsPointers.Add(new MySQLColumnPointer(ProjectPatternsList[j].GetHashCode(), patternItemElementName, text3, MySQLColumnPointerType.AttributeData));
    					if (ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes[l].IsWeMustDownloadContentUnderAttributeLink)
    					{
    						string text4 = string.Format("{0}_attrSavedData", text2, PrepareColumnName(ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes[l].AttributeName));
    						text += $"\t`{text4}` TEXT NULL,\r\n";
    						string patternItemElementName2 = $"Attribute-{ProjectPatternsList[j].GetHashCode()}-{ProjectPatternsList[j].GrabbingItemsPatterns[k].ElementName}-{ProjectPatternsList[j].GrabbingItemsPatterns[k].ParsingBinaryAttributes[l].AttributeName}-SavedFiles";
    						DataColumnsPointers.Add(new MySQLColumnPointer(ProjectPatternsList[j].GetHashCode(), patternItemElementName2, text4, MySQLColumnPointerType.AttributeFileName));
    					}
    				}
    			}
    		}
    		text += "\tPRIMARY KEY (`Id`));\r\n\r\n";
    		try
    		{
    			File.WriteAllText(this.FileSavingPath, text);
    		}
    		catch (Exception occuredException)
    		{
    			TaskLoggerLink.Log(DataExcavatorTasksLoggerEntityType.DataExporter, $"Can't initialize mysql exporter - can't write to file {this.FileSavingPath}", occuredException);
    		}
    	}

    	/// <summary>
    	/// Prepares column name to database insert macro. Converts name to latin characters.
    	/// </summary>
    	/// <param name="ColumnName">Dirty column name with possibly uncorrect characters</param>
    	/// <returns>Prepared and cleared column name</returns>
    	private string PrepareColumnName(string ColumnName)
    	{
    		ColumnName = TextTransliterations.TransliterateFromCyryllicToLatin(ColumnName);
    		ColumnName = ColumnName.NormalizeStringToCharactersAndNumbers();
    		return ColumnName;
    	}

    	/// <summary>
    	/// Prepares value to DB insert
    	/// </summary>
    	/// <param name="InsertingValue"></param>
    	/// <returns></returns>
    	private string ProcessDBValue(string InsertingValue)
    	{
    		if (InsertingValue.IndexOf("'") != -1)
    		{
    			InsertingValue = InsertingValue.Replace("'", "|");
    		}
    		return InsertingValue;
    	}
    }
}
