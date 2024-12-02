using API.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RaceController : ControllerBase {

        [HttpPost("New")]
        [RequestFormLimits(MultipartBodyLengthLimit = 20_000_000, ValueLengthLimit = 20_000_000)]
        public ActionResult AddRace(IFormFile file) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            //foreach (IFormFile file in Request.Form.Files) {
                string fullPath = Path.Combine(pathToSave, file.FileName);
                using FileStream stream = new(fullPath, FileMode.Create);
                file.CopyTo(stream);
            //}

            stream.Close();

            using (PdfDocument document = PdfDocument.Open(fullPath)) {
                foreach (Page page in document.GetPages()) {
                    var words = page.GetWords().OrderBy(x => x.BoundingBox.Left).GroupBy(x => x.BoundingBox.Bottom, new ToleranceEqualityComparer());

                    StringBuilder builder = new StringBuilder();

                    foreach (var word in words) {
                        foreach (var item in word) {
                            builder.Append(item.Text + " - ");
                        }
                        builder.Append("\n");
                    }

                    string text = builder.ToString();

                    Console.WriteLine(text);

                    //List<Word> words = page.GetWords().ToList();
                    //List<int> toMerge = new();
                    //for (int i = 0; i < words.Count() - 1; i++) {
                    //    Word current = words[i];
                    //    Word next = words[i + 1];

                    //    if (current.BoundingBox.Right + 5 < next.BoundingBox.Left) {
                    //        toMerge.Add(i);
                    //    }
                    //}
                }
            }


            //PdfReader reader = new PdfReader(System.IO.File.ReadAllBytes(fullPath));

            //for (var pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++) {
            //    // Get the page content and tokenize it.
            //    var contentBytes = reader.GetPageContent(pageNum);
            //    var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(contentBytes));

            //var stringsList = new List<string>();
            //    while (tokenizer.NextToken()) {
            //        if (tokenizer.TokenType == PRTokeniser.TokType.STRING) {
            //            // Extract string tokens.
            //            stringsList.Add(tokenizer.StringValue);
            //        }
            //    }

            //    // Print the set of string tokens, one on each line.
            //    Console.WriteLine(string.Join("    ", stringsList));
            //}

            //reader.Close();

            Console.WriteLine(file.FileName);// we can put rest of upload logic here.
            return Ok();
        }
    }
}
