namespace Poe.Models;

using System;

/**
 * Utilized for the manipulation of text across the application
 * Functions such as finding a position within the document,
 * inserting, deleting, searching, and replacing text.
 */
public abstract class Text(string content)
{
    /**
     * Length of content
     */
    public int Length => content.Length;

    /**
     * Find the position of a substring within the text
     */
    public int FindPosition(string substring)
    {
        return content.IndexOf(substring, StringComparison.Ordinal);
    }

    /**
     * Insert text at a given position
     */
    public void Insert(int position, string textToInsert)
    {
        if (position < 0 || position > content.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        content = content.Insert(position, textToInsert);
    }

    /**
     * Delete a range of text
     */
    public void Delete(int startIndex, int length)
    {
        if (startIndex < 0 || length < 0 || startIndex + length > content.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        content = content.Remove(startIndex, length);
    }

    /**
     * Replace a range of text with a new string
     */
    public void Replace(int startIndex, int length, string newText)
    {
        Delete(startIndex, length);
        Insert(startIndex, newText);
    }

    /**
     * Search for all instances of a substring
     */
    public abstract int[] Search(string substring);

    /**
     * Optionally, provide a way to retrieve the current text state
     */
    public string Content => content;
}