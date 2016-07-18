using CarRent.DataAccess.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CarRent.Extensions
{
    public static class ControllerExtensions
    {
        public static void AddErrorFromResult(this Controller ctrl, DataResult result)
        {
            foreach (var error in result.Errors)
            {
                ctrl.ModelState.AddModelError("", error.Description);
            }
        }

        public static IActionResult GetErrorResult(this Controller ctrl, DataResult result)
        {
            if (result == null)
            {
                return  new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ctrl.ModelState.AddModelError("", error.Description);
                    }
                }

                if (ctrl.ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return ctrl.HttpBadRequest();
                }
                return ctrl.HttpBadRequest(ctrl.ModelState);
            }

            return null;
        }
    }
}
