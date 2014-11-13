using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Irony.Samples {
  // A sample grammar for reading comma-separated file containing cars information
  // Demonstrates use of DsvLiteral. Use sample data file DataFiles\CarModels.csv.
  // The data is copied from Wikipedia article about CSV format: http://en.wikipedia.org/wiki/Comma-separated_values
  [Language("CarListing-CSV", "1.0", "Grammar for reading CSV files.")]
  public class SampleCsvGrammar : Grammar {
    public SampleCsvGrammar() {
      this.GrammarComments = 
@"A sample grammar for reading comma-separated file containing cars information
Demonstrates use of DsvLiteral. Use sample data file DataFiles\CarModels.csv.";
      //Terminals
      var year = new DsvLiteral("Year", TypeCode.Int16);  // comma is default separator
      var maker = new DsvLiteral("Maker", TypeCode.String);
      var model = new DsvLiteral("Model", TypeCode.String);
      var comment = new DsvLiteral("Comment", TypeCode.String);
      var price = new DsvLiteral("Price", TypeCode.Double, null);  //null means no terminator, look for NewLine 

      var line = new NonTerminal("Line");
      var lines = new NonTerminal("Lines"); 
      var data = new NonTerminal("data"); 

      //Rules
      line.Rule = year + maker + model + comment + price + NewLine; //we don't specify comma between fields, because our terminals consume separator automatically
      lines.Rule = MakeStarRule(lines, line); 
      data.Rule = lines + NewLineStar; //to allow empty lines after
      this.Root =  data; 
      this.LanguageFlags |= LanguageFlags.NewLineBeforeEOF; 

    }//constructor
  }//class
}//namespace
