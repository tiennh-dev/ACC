using Core.Common;
using Ichiba.Cdn.Model;
using iChibaShopping.Core.AppService.Interface;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace iChiba.OM.PrivateApi.Controllers
{
    public class FileController : BaseController
    {
        protected static readonly FormOptions DefaultFormOptions = new FormOptions();
        private readonly IFileAppService fileAppService;

        public FileController(ILogger<FileController> logger,
            IFileAppService fileAppService)
            : base(logger)
        {
            this.fileAppService = fileAppService;
        }

        [RequestSizeLimit(100000000)]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FileUploadResponse))]
        public async Task<IActionResult> UploadImage()
        {
            try
            {
                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), DefaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = reader.ReadNextSectionAsync().Result;
                while (section != null)
                {
                    ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        var fileNameUpload = HeaderUtilities.RemoveQuotes(contentDisposition.FileName).Value;
                        var bytes = StreamToBytes(section.Body);
                        var result = await fileAppService.Upload(fileNameUpload, bytes);

                        return Ok(result);
                    }
                    section = await reader.ReadNextSectionAsync();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }

        private byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
