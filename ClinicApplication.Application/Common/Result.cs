using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.Common
{

    public enum ResultStatus { Success, NotFound, BadRequest, Conflict, Unauthorized }
    public class Result
    {
        public ResultStatus Status { get; private set; }
        public string? Message { get; private set; }
        public int? Id { get; private set; }

        public static Result Success() => new() { Status = ResultStatus.Success };
        public static Result Success(int id) => new() { Status = ResultStatus.Success, Id = id };
        public static Result NotFound(string msg) => new() { Status = ResultStatus.NotFound, Message = msg };
        public static Result BadRequest(string msg) => new() { Status = ResultStatus.BadRequest, Message = msg };
        public static Result Conflict(string msg) => new() { Status = ResultStatus.Conflict, Message = msg };
        public static Result Unauthorized(string msg) => new() { Status = ResultStatus.Unauthorized, Message = msg };
    }

    public class Result<T>
    {
        public ResultStatus Status { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }

        public static Result<T> Success(T data) => new() { Status = ResultStatus.Success, Data = data };
        public static Result<T> NotFound(string msg) => new() { Status = ResultStatus.NotFound, Message = msg };
        public static Result<T> BadRequest(string msg) => new() { Status = ResultStatus.BadRequest, Message = msg };
        public static Result<T> Conflict(string msg) => new() { Status = ResultStatus.Conflict, Message = msg };
        public static Result<T> Unauthorized(string msg) => new() { Status = ResultStatus.Unauthorized, Message = msg };
    }
}
