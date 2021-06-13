using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.ViewModel
{
    public class JsonResultViewModel
    {
        public JsonResultViewModel(ModelStateDictionary modelState) : this()
        {
            Success = false;
            Errors = modelState.Where(r => r.Value.Errors.Count > 0).ToList().Select(r => new ErrorObj { Name = r.Key, Value = r.Value.Errors.First().ErrorMessage }).ToList();
        }

        public JsonResultViewModel(IEnumerable<KeyValuePair<string, string>> errors) : this()
        {
            Success = false;
            Errors = errors.ToList().Select(r => new ErrorObj { Name = r.Key, Value = r.Value }).ToList();
        }

        public JsonResultViewModel(IEnumerable<IdentityError> errors) : this()
        {
            Success = false;
            Errors = errors.Select(r => new ErrorObj { Name = r.Code, Value = r.Description }).ToList();
        }

        public JsonResultViewModel(Exception ex) : this()
        {
            Success = false;
            Errors = new List<ErrorObj> {
                new ErrorObj{ Exception=ex.Message}
            };
        }

        public JsonResultViewModel(object vm) : this()
        {
            this.Success = true;
            this.CustomResult = vm;

        }

        public JsonResultViewModel()
        {
            this.Errors = new List<ErrorObj>();
        }

        public void AddError(string exception = null, string name = null, string value = null)
        {
            this.Errors.Add(new ErrorObj
            {
                Exception = exception,
                Name = name,
                Value = value,
            });
        }


        public string Message { get; set; }
        public bool Success { get; set; }
        public int Staus { get; set; }
        public string Exception { get; set; }
        public List<ErrorObj> Errors { get; set; } = new List<ErrorObj>();
        public object CustomResult { get; set; }
        public class ErrorObj
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Exception { get; set; }
        }
    }
}
