﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NotificationConfiguration
{
    /// <summary>
    /// Performs rudimentary parsing of a CODEOWNERS file for synchornizing into 
    /// </summary>
    /// <remarks>
    /// See: https://github.com/Azure/azure-sdk/blob/master/docs/engineering-system/codeowners.md
    /// </remarks>
    public class CodeOwnersParser
    {
        private class ExpressionContacts
        {
            public string GlobExpression { get; set; }
            public List<string> Contacts { get; set; }
        }

        private readonly List<ExpressionContacts> expressionContacts;
                           
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileContents"></param>
        public CodeOwnersParser(string fileContents)
        {
            expressionContacts = GetExpressions(fileContents);
        }

        /// <summary>
        /// Returns GitHub contacts for a given file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> GetContactsForPath(string path)
        {
            return expressionContacts
                .LastOrDefault(expr => path.StartsWith(expr.GlobExpression))
                ?.Contacts;
        }

        /// <summary>
        /// Returns a list of expressions and contacts from a given file's contents
        /// </summary>
        /// <remarks> 
        /// Some of the behavior comes from the format described at: 
        /// https://git-scm.com/docs/gitignore#_pattern_format
        /// </remarks>
        /// <returns>List of ExpressionContacts in file order</returns>
        private List<ExpressionContacts> GetExpressions(string fileContents)
        {
            var fileLines = fileContents
                .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Trim())
                .Where(entry => entry != string.Empty)      // Exclude empty lines
                .Where(entry => !entry.StartsWith("#"));    // Exclude comments

            return fileLines
                .Select(line => 
                {
                    var splitLine = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    var result = new ExpressionContacts
                    {
                        GlobExpression = splitLine[0],
                        Contacts = splitLine.Skip(1).ToList(),
                    };
                    return result;
                }).ToList();
                
        }
    }
}
