using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO; //Input and output

namespace ShapeShift
{
    public class FileManager
    {

        enum LoadType { Attributes, Contents };
        LoadType type;

        //TwoDimensional List
        //Note: to add to a 2D list, you have to add another list
        //if we are loading in more than one attribute, etc.
        //if we look at it like an array
        //attributes[line][element of the line]
        //However, it has no set limit like an array (which is why we are working with them)

        List<string> tempAttributes;
        List<string> tempContents;

        bool identifierFound = false;

        public void LoadContent(string filename, List<List<string>> attributes, List<List<string>> contents)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();//reads a line in the text file

                    //Now we check to see if we are loading an attribute, or contents
                    if (line.Contains("Load="))
                    {
                        tempAttributes = new List<string>();
                        line = line.Remove(0, line.IndexOf("=") + 1);//we are gonna remove the 'Load='
                        type = LoadType.Attributes;

                    }
                    else 
                    {
                        //tempContents = new List<string>();
                        type = LoadType.Contents;
                    }

                    tempContents = new List<string>();
                    //In the text file, there are '[' and ']' symbols. We need to use these to distinguish the next item
                    string[] lineArray = line.Split(']'); // an array of the split lines
                   
                    foreach (string li in lineArray) //'li' being an abbreviation for the word 'line'
                    { 
                    string newLine = li.Trim('[',' ',']');
                    if (newLine != String.Empty)
                    {
                        if (type == LoadType.Contents)
                            tempContents.Add(newLine);
                        else
                            tempAttributes.Add(newLine);
                    }
                    
                    }//end foreach

                    if (type == LoadType.Contents && tempContents.Count > 0) //deals with empty space
                    {
                        contents.Add(tempContents);
                        attributes.Add(tempAttributes);
                    }

                }
            }
        
        }


        //Slight difference: 
        //if it identifies the 'endload' identifier, then it ends loading
        //if it identifies the 'startload' identifier, then it starts loading
        public void LoadContent(string filename, List<List<string>> attributes, List<List<string>> contents,string identifier)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();//reads a line in the text file

                    if(line.Contains("EndLoad=") && line.Contains(identifier))
                    {
                        identifierFound = false;
                        break;
                    }
                    else if(line.Contains("Load=") && line.Contains(identifier))
                    {
                       // Console.WriteLine("LINE: " + line);
                    identifierFound = true;
                        continue; // our identifier is not an attribute, we need to continue to the attribute
                    }

                    if (identifierFound)
                    {
                        //Now we check to see if we are loading an attribute, or contents
                        if (line.Contains("Load="))
                        {
                            tempAttributes = new List<string>();
                            line = line.Remove(0, line.IndexOf("=") + 1);//we are gonna remove the 'Load='
                            type = LoadType.Attributes;

                        }
                        else
                        {
                            tempContents = new List<string>();
                            type = LoadType.Contents;
                        }

                        //In the text file, there are '[' and ']' symbols. We need to use these to distinguish the next item
                        string[] lineArray = line.Split(']'); // an array of the split lines

                        foreach (string li in lineArray) //'li' being an abbreviation for the word 'line'
                        {
                            string newLine = li.Trim('[', ' ', ']');
                            if (newLine != String.Empty)
                            {
                                if (type == LoadType.Contents)
                                    tempContents.Add(newLine);
                                else
                                    tempAttributes.Add(newLine);
                                Console.WriteLine("newLine: " + newLine);
                            }

                        }//end foreach

                        if (type == LoadType.Contents && tempContents.Count > 0) //deals with empty space
                        {
                            contents.Add(tempContents);
                            attributes.Add(tempAttributes);
                        }

                    }
                }
            }
        }

    }
}
