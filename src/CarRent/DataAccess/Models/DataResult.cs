using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public class DataResult
    {
        public IEnumerable<DataError> Errors { get; protected set; }

        public bool Succeeded { get; protected set; }

        /// <summary>
        /// Failed helper method
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static DataResult Failed(params DataError[] errors)
        {
            return new DataResult() { Errors = errors, Succeeded = false };
        }

        /// <summary>
        /// Success hellper method
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataResult Succeed()
        {
            return new DataResult() { Succeeded = true };
        }
    }

    public class DataResult<T>: DataResult
    {
        public T Data { get; protected set; }

        /// <summary>
        /// Failed helper method
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public new static DataResult<T> Failed(params DataError[] errors)
        {
            return new DataResult<T>() { Errors = errors, Succeeded = false };
        }

        /// <summary>
        /// Success hellper method
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataResult<T> Succeed(T data)
        {
            return new DataResult<T>() { Data = data, Succeeded = true };
        }
       
    }
}
