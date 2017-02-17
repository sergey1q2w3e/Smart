using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Azure.Mobile.Server;
using MySmartHouse1Service.DataObjects;
using MySmartHouse1Service.Models;

namespace MySmartHouse1Service.Controllers
{
    public class ParametersController : TableController<Parameters>
    {
        private MySmartHouse1Context db = new MySmartHouse1Context();

        // GET: api/Parameters
        public IQueryable<Parameters> GetParametrs()
        {
            return db.Parametrs;
        }

        // GET: api/Parameters/5
        [ResponseType(typeof(Parameters))]
        public IHttpActionResult GetParameters(string id)
        {
            Parameters parameters = db.Parametrs.Find(id);
            if (parameters == null)
            {
                return NotFound();
            }

            return Ok(parameters);
        }

        // PUT: api/Parameters/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutParameters(string id, Parameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parameters.Id)
            {
                return BadRequest();
            }

            db.Entry(parameters).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParametersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Parameters
        [ResponseType(typeof(Parameters))]
        public IHttpActionResult PostParameters(Parameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Parametrs.Add(parameters);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ParametersExists(parameters.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = parameters.Id }, parameters);
        }

        // DELETE: api/Parameters/5
        [ResponseType(typeof(Parameters))]
        public IHttpActionResult DeleteParameters(string id)
        {
            Parameters parameters = db.Parametrs.Find(id);
            if (parameters == null)
            {
                return NotFound();
            }

            db.Parametrs.Remove(parameters);
            db.SaveChanges();

            return Ok(parameters);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ParametersExists(string id)
        {
            return db.Parametrs.Count(e => e.Id == id) > 0;
        }
    }
}