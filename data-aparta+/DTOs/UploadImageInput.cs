using HotChocolate.Types;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class FileUploadInput
    {
        [GraphQLType(typeof(NonNullType<UploadType>))]
        public IFile File { get; set; }
        public string Type { get; set; }
        public string PropertyId { get; set; }
    }

    public class FileUploadResponse
    {
        public string Url { get; set; }
        public string Key { get; set; }
    }
}
