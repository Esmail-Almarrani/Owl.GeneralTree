﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Owl.GeneralTree;

public class GeneralTreeCodeGenerator : IGeneralTreeCodeGenerator
{
    private readonly GeneralTreeCodeGeneratorOptions _generalTreeCodeGeneratorOptions;

    public GeneralTreeCodeGenerator(IOptions<GeneralTreeCodeGeneratorOptions> generalTreeCodeGeneratorOptionsAccessor)
    {
        _generalTreeCodeGeneratorOptions = generalTreeCodeGeneratorOptionsAccessor.Value;
    }

    /// <summary>
    /// Creates code for given numbers.
    /// Example: if numbers are 1,2 then returns "00001.00002";
    /// </summary>
    /// <param name="numbers">Numbers</param>
    public string CreateCode(params int[] numbers)
    {
        return numbers.IsNullOrEmpty()
            ? null
            : numbers.Select(number => number.ToString(new string('0', _generalTreeCodeGeneratorOptions.CodeLength))).JoinAsString(".");
    }

    /// <summary>
    /// Merge a child code to a parent code.
    /// Example: if parentCode = "00001", childCode = "00002" then returns "00001.00002".
    /// </summary>
    /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
    /// <param name="childCode">Child code.</param>
    public string MergeCode(string parentCode, string childCode)
    {
        if (childCode.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
        }

        if (parentCode.IsNullOrEmpty())
        {
            return childCode;
        }

        return parentCode + "." + childCode;
    }

    /// <summary>
    /// Merge a child FullName to a parent FullName. Same with <exception cref="MergeCode"></exception>
    /// Example: if parentFullName = "00001", childFullName = "00002" then returns "00001-00002".
    /// </summary>
    /// <param name="parentFullName">Parent FullName. Can be null or empty if parent is a root.</param>
    /// <param name="childFullName">Child FullName.</param>
    /// <param name="hyphen">hyphen</param>
    public string MergeFullName(string parentFullName, string childFullName, string hyphen)
    {
        if (childFullName.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(childFullName), "childFullName can not be null or empty.");
        }

        if (parentFullName.IsNullOrEmpty())
        {
            return childFullName;
        }

        return parentFullName + hyphen + childFullName;
    }

    /// <summary>
    /// Remove the parent code
    /// Example: if code = "00001.00002.00003" and parentCode = "00001" then returns "00002.00003".
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="parentCode">The parent code.</param>
    public string RemoveParentCode(string code, string parentCode)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        if (parentCode.IsNullOrEmpty())
        {
            return code;
        }

        return code.Length == parentCode.Length ? null : code.Substring(parentCode.Length + 1);
    }

    public string RemoveParentCode(string code, int parentLevel)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var codeArray = code.Split(".");
        if (parentLevel > codeArray.Length)
        {
            throw new ArgumentNullException(nameof(parentLevel), "parentLevel exceeds the level of code.");
        }

        return string.Join(".", codeArray.Skip(parentLevel));
    }

    public string RemoveParentFullName(string fullName, string parentFullName, string hyphen)
    {
        if (fullName.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(fullName), "fullName can not be null or empty.");
        }

        if (parentFullName.IsNullOrEmpty())
        {
            return fullName;
        }

        return fullName.Length == parentFullName.Length ? null : fullName.Substring(parentFullName.Length + hyphen.Length);
    }

    public string RemoveParentFullName(string fullName, int parentLevel, string hyphen)
    {
        if (fullName.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(fullName), "fullName can not be null or empty.");
        }

        var codeArray = fullName.Split(hyphen);
        if (parentLevel > codeArray.Length)
        {
            throw new ArgumentNullException(nameof(parentLevel), "parentLevel exceeds the level of code.");
        }

        return string.Join(hyphen, codeArray.Skip(parentLevel));
    }

    /// <summary>
    /// Get next code for given code.
    /// Example: if code = "00001.00001" returns "00001.00002".
    /// </summary>
    /// <param name="code">The code.</param>
    public string GetNextCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var parentCode = GetParentCode(code);
        var lastUnitCode = GetLastCode(code);
        return MergeCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
    }

    /// <summary>
    /// Gets the last code.
    /// Example: if code = "00001.00002.00003" returns "00003".
    /// </summary>
    /// <param name="code">The code.</param>
    public string GetLastCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var splitCode = code.Split('.');
        return splitCode[splitCode.Length - 1];
    }

    /// <summary>
    /// Gets parent code.
    /// Example: if code = "00001.00002.00003" returns "00001.00002".
    /// </summary>
    /// <param name="code">The code.</param>
    public string GetParentCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var splitCode = code.Split('.');
        return splitCode.Length == 1 ? null : splitCode.Take(splitCode.Length - 1).JoinAsString(".");
    }
}