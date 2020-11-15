﻿using UnityEditor;

/// <summary>
/// In Unity 2020 the old way (https://github.com/zaikman/unity-editorconfig)
/// of inserting the editorConfig-File into the visual studio solution file now longer works as described here:
/// https://forum.unity.com/threads/bug-unity-2020-1-enable_vstu-define-is-gone-and-projectfilegeneration-event-isnt-raised-anymore.942664/
/// This script will fill the gap.
/// 
/// Requirements:
/// * Unity 2020 with, "Visual Studio Editor".Package in Projekt at least 2.0.5.
/// * I use VS2019 16.8.1 with VisualStudioTools for Unity.
/// </summary>
/// <remarks>https://docs.microsoft.com/en-us/visualstudio/ide/editorconfig-code-style-settings-reference?view=vs-2017.</remarks>
public class EditorConfigPostprocessor : AssetPostprocessor
{
    /// <summary>
    /// This term is exactly at the spot between project references and the global section.
    /// </summary>
    private const string FindString = "EndProject\r\nGlobal";

    /// <summary>
    /// The replacement term needs to include the parts removed by <see cref="FindString"/>.
    /// the .editorConfig is added on the solution level.
    /// The GUID "2150E333-8FDC-42A3-9474-1A3956D46DE8" is a constant for "Solution Folder" items.
    /// https://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
    /// The GUID "B24FE069-BB5F-4F16-BCDA-61C28EABC46B" is a random identifier für the file.
    /// </summary>
    private const string ReplaceString =
        "EndProject\r\n" +
        "Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", " +
        "\"{B24FE069-BB5F-4F16-BCDA-61C28EABC46B}\"\r\n"+
        "	ProjectSection(SolutionItems) = preProject\r\n" +
        "		.editorconfig = .editorconfig\r\n" +
        "	EndProjectSection\r\n" +
        "EndProject\r\n" +
        "Global";

    /// <summary>
    /// Called when a solutionfile (*sln) is modified.
    /// </summary>
    /// <param name="path">Path to the SLN file.</param>
    /// <param name="content">The content of the SLN file. In microsofts "xml without brackets" format.</param>
    /// <returns>The content used as the created or modified SLN file.</returns>
    public static string OnGeneratedSlnSolution(string path, string content)
    {
        // As the file is modified, not neccesarily created, check if there already is an entry for SolutionItems. 
        if (!content.Contains("2150E333-8FDC-42A3-9474-1A3956D46DE8"))
        {
            content = content.Replace(FindString, ReplaceString);
        }

        return content;
    }

    /// <summary>
    /// Called when a projectfile (*.csproj) is modified.
    /// </summary>
    /// <param name="path">Path to the CSPROJ file.</param>
    /// <param name="content">The content of the file. Xml.</param>
    /// <returns>The content used as the created or modified projectfile.</returns>
    public static string OnGeneratedCSProject(string path, string content)
    {
        return content;
    }
}
