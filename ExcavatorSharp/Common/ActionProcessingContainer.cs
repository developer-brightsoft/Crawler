// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// ExcavatorSharp, Version=1.2.8.0, Culture=neutral, PublicKeyToken=null
// ExcavatorSharp.Common.ActionProcessingContainer
using System;

/// <summary>
/// Common processing container for any background work
/// </summary>
namespace ExcavatorSharp.Common
{
	public class ActionProcessingContainer
	{
		/// <summary>
		/// Actual processing value
		/// </summary>
		public int ActualProcessPointer { get; set; }

		/// <summary>
		/// Total entries to process
		/// </summary>
		public int TotalDataToProcess { get; set; }

		/// <summary>
		/// Actual completion percent
		/// </summary>
		public double CompletionPercent { get; set; }

		/// <summary>
		/// Outgoing message (log data or common information)
		/// </summary>
		public string OutputMessage { get; set; }

		/// <summary>
		/// Creates new instance of ActionProcessingContainer
		/// </summary>
		/// <param name="ActualProcessPointer">Actual processing value</param>
		/// <param name="TotalDataToProcess">Total entries to process</param>
		/// <param name="OutputMessage">Outgoing message (log data or common information)</param>
		public ActionProcessingContainer(int ActualProcessPointer, int TotalDataToProcess, string OutputMessage = "")
		{
			CompletionPercent = Math.Round((double)ActualProcessPointer / (double)TotalDataToProcess * 100.0, 2);
			this.ActualProcessPointer = ActualProcessPointer;
			this.TotalDataToProcess = TotalDataToProcess;
			CompletionPercent = CompletionPercent;
			this.OutputMessage = OutputMessage;
		}
	}
}
