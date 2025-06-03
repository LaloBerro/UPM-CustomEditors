using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.CustomTools
{
    public static class GitPackagesWithBranchSearcher
    {
        private const string ManifestPath = "Packages/manifest.json";

        [MenuItem("Tools/Search Git Packages with Branch")]
        public static void CheckGitPackages()
        {
            if (!File.Exists(ManifestPath))
            {
                Debug.LogError("manifest.json not found at: " + ManifestPath);
                return;
            }

            string manifestJson = File.ReadAllText(ManifestPath);

            Dictionary<string, string> gitPackages = FindGitPackages(manifestJson);

            if (gitPackages.Count == 0)
            {
                Debug.Log("No Git packages found in manifest.");
                return;
            }

            foreach (var package in gitPackages)
            {
                string branch = GetGitBranchFromUrl(package.Value);
                if (!string.IsNullOrEmpty(branch))
                {
                    Debug.Log($"Package: <b>{package.Key}</b> is on branch: <color=green>{branch}</color>");
                }
                else
                {
                    Debug.LogWarning($"Package: <b>{package.Key}</b> is from Git but no branch specified.");
                }
            }
        }

        private static Dictionary<string, string> FindGitPackages(string json)
        {
            var result = new Dictionary<string, string>();
            
            Regex regex = new Regex("\"([^\"]+)\"\\s*:\\s*\"(https:[^\"]+\\.git(?:#[^\"]+)?)\"");
            MatchCollection matches = regex.Matches(json);

            foreach (Match match in matches)
            {
                if (match.Groups.Count != 3) 
                    continue;
                
                string name = match.Groups[1].Value;
                string url = match.Groups[2].Value;

                if (url.Contains(".git"))
                    result[name] = url;
            }

            return result;
        }

        private static string GetGitBranchFromUrl(string url)
        {
            int hashIndex = url.IndexOf("#", StringComparison.Ordinal);
            string gitBranch = hashIndex >= 0 && hashIndex < url.Length - 1
                ? url.Substring(hashIndex + 1)
                : null;
            
            return gitBranch;
        }
    }
}