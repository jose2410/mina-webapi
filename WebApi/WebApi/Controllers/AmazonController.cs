using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("amazon")]
    public class AmazonController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private const string AccessKey = "AKIA4BVRNQHZBTYMQQEY";
        private const string SecretKey = "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";
        private const string BucketName = "muchabucket";
        private const string FolderName = "pdfs";

        public AmazonController()
        {
            var credentials = new Amazon.Runtime.BasicAWSCredentials(AccessKey, SecretKey);
            _s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1); // Reemplaza YOUR_REGION por la región correspondiente

        }

        [HttpPost("upload-pdf")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo inválido");

            if (Path.GetExtension(file.FileName) != ".pdf")
                return BadRequest("El archivo debe ser un PDF");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var uploadRequest = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = $"{FolderName}/{file.FileName}",
                    //Key = $"{FolderName}/{Guid.NewGuid()}.pdf", //
                    InputStream = memoryStream
                };
                await _s3Client.PutObjectAsync(uploadRequest);
            }

            return Ok("Archivo PDF subido exitosamente");
        }

        [HttpGet("list-pdfs")]
        public async Task<IActionResult> ListPdfs()
        {
            var listRequest = new ListObjectsRequest
            {
                BucketName = BucketName,
                Prefix = FolderName
                //Prefix = $"{FolderName}/"
            };
            var response = await _s3Client.ListObjectsAsync(listRequest);

            var pdfFileNames = response.S3Objects
                .Where(obj => Path.GetExtension(obj.Key) == ".pdf")
                .Select(obj => new PDFInfo
                {
                    FileName = Path.GetFileNameWithoutExtension(obj.Key.Substring(5)),
                    FilePath = obj.Key
                }).ToList();

            return Ok(pdfFileNames);
        }

        //buscar archivo
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarArchivo(string nombre)
        {

            var request = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = $"{FolderName}/{nombre}.pdf"
            };

            try
            {
                //var response = await _s3Client.GetObjectAsync(request);

                //using var responseStream = response.ResponseStream;
                //using var reader = new StreamReader(responseStream);
                //var contenido = await reader.ReadToEndAsync();

                //return Ok(contenido);

                //otra forma
                var response = await _s3Client.GetObjectAsync(request);

                // Obtener el nombre del archivo sin la extensión .pdf
                string nombreArchivo = Path.GetFileNameWithoutExtension(nombre);

                return Ok(nombreArchivo);
            }
            catch (AmazonS3Exception e)
            {
                return NotFound("El archivo no se encontró: " + e.Message);
            }
        }

        [HttpGet("descargar")]
        public async Task<IActionResult> DescargarArchivo(string nombre)
        {

            var request = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = $"{FolderName}/{nombre}.pdf"
            };

            try
            {
                //var response = await _s3Client.GetObjectAsync(request);

                //using var responseStream = response.ResponseStream;
                //var mimeType = "application/pdf"; // Establece el tipo MIME a "application/pdf" para archivos PDF
                //return File(responseStream, mimeType, $"{nombre}.pdf");
                using var response = await _s3Client.GetObjectAsync(request);
                using var responseStream = response.ResponseStream;

                if (responseStream != null)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await responseStream.CopyToAsync(memoryStream);

                    return File(memoryStream.ToArray(), "application/pdf", $"{nombre}.pdf");
                }
                else
                {
                    return NotFound("El archivo no se encontró en Amazon S3.");
                }
            }
            catch (AmazonS3Exception e)
            {
                return NotFound("El archivo no se encontró: " + e.Message);
            }
        }

    }
}