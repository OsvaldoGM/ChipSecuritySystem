using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChipSecuritySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chip Security System!");
            Console.WriteLine("Please enter your bag of Chips with the next structure as an example: '[Blue, Yellow] [Red, Green] [Yellow, Red] [Orange, Purple]'");

            //We read the sequence of the Chips raw, and inmediatly we need to parse them to create a list of ColorChips
            string bagOfChipsRawStr = Console.ReadLine();
            Console.WriteLine("I got this chips: " + bagOfChipsRawStr);

            List<ColorChip> bagOfChips = parseBagOfChipsRaw(bagOfChipsRawStr);

            //We need a copy Deep of the BagOfChips so we can remove the Chips that we are using in the sequence
            List<ColorChip> tempChip = new List<ColorChip>();
            foreach (var chip in bagOfChips)
            {
                tempChip.Add(chip);
            }

            //Here we are going to dump the right sequence
            List<ColorChip> solutionChips = new List<ColorChip>();

            foreach (var chip in bagOfChips)
            {

                if (solutionChips.Count<1)
                {
                    //We always start with the Blue chip
                    ColorChip colorChip = tempChip.Where(tChip => tChip.StartColor == Color.Blue).First();
                    tempChip.Remove(colorChip);
                    solutionChips.Add(colorChip);

                }else{
                    
                    //For each subsequent we grab the last chip added to the solution list (index-1), we compare how that chip color ends
                    //and we need to find a chip that starts with that color
                    ColorChip colorChip = tempChip.Where(tChip => tChip.StartColor == solutionChips[solutionChips.Count-1].EndColor).FirstOrDefault();

                    if (colorChip == null)
                    {
                        //We need to see if there is another Colorchip that starts with the same color that can help us to make the connection
                        ColorChip otherColorChip = tempChip.Where(tChip => tChip.StartColor == solutionChips[solutionChips.Count-1].StartColor).FirstOrDefault();
                        if(otherColorChip == null){
                            //If we dont find a alternative color Chip that also starts with the same color, we ran out of connections
                            Console.WriteLine(Constants.ErrorMessage);
                            System.Environment.Exit(1);
                        }

                        //If we manage to found it, we need to remove the ColorChip without conexion, and use the new ColorChip
                        solutionChips.Remove(solutionChips[solutionChips.Count-1]);
                        colorChip = otherColorChip;
                    }

                    //If we found a conexion or have a new chip to work with, we need to remove it from out Temporary List and added to our Solution List
                    tempChip.Remove(colorChip);
                    solutionChips.Add(colorChip);

                    //If we ever arrived to a chip that the color ends with Green, then the connection is a success!
                    if (colorChip.EndColor == Color.Green)
                    {
                        Console.WriteLine($"Correct Sequence found!");

                        foreach(var chipSol in solutionChips)
                        {
                            Console.WriteLine($"[{chipSol}]");

                        }

                        Console.WriteLine($"Master panel unlocked!");
                        System.Environment.Exit(1);
                    }

                }

            }


            Console.WriteLine(Constants.ErrorMessage);
            System.Environment.Exit(1);

        }

        private static List<ColorChip> parseBagOfChipsRaw(string bagOfChipsRawStr){
            List<ColorChip> bagOfChips = new List<ColorChip>();
            //We split the string every ']' so we can get the N amount of Chips
            var arrStrChips = bagOfChipsRawStr.Split(']');


            foreach(string ch in arrStrChips)
            {
                if (!string.IsNullOrEmpty(ch))
                { 
                    //We replace the other side of the array '[' and Trim so we dont get any blank spaces that could lead to errors
                    //Then for each string of color we the Color object, then we need to check if we dont have it already on the list
                    //Note** I would typically use HashSet in this cases to ensure no duplicate, but they were not working in this case, and I rather turn a working code that run of time
                    var chipSplit = ch.Replace("[", "").Trim();
                    var chipColors = chipSplit.Split(',');
                    Color color1 = getEnumColor(chipColors[0]);
                    Color color2 = getEnumColor(chipColors[1]);
                    ColorChip colorChip = new ColorChip(color1, color2);
                    if(!containsColorChip(bagOfChips, colorChip)){
                        bagOfChips.Add(colorChip);
                    }
                }
            }


            return bagOfChips;
        }

        private static bool containsColorChip(List<ColorChip> bagOfChips, ColorChip colorChip)
        {
            //Here we just compare if the Star and End color are the same, in that case we are looking at a duplicate
            foreach (var ch in bagOfChips)
            { 
                bool startColorSame = colorChip.StartColor.Equals(ch.StartColor);
                bool endColorSame = colorChip.EndColor.Equals(ch.EndColor);

                if (startColorSame && endColorSame) {
                    return true;
                }
            }

            return false;
        }

        private static Color getEnumColor(string color)
        {
            Color colorFound = new Color();
            //We need to parse the string of the color to compare it to the Enum value, so we can return the Color object, if we dont found any
            //, it will throw and exception, so we catch the exception showing a message that the color dosent exists and then we exit the program
            try
            {
                colorFound = (Color)Enum.Parse(typeof(Color), color);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exiting the program! Could not found color {color}");
                System.Environment.Exit(1);
            }


            return colorFound;
        }
    }
}
