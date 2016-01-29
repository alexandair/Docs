using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using MVCMovie.Models;

namespace MVCMovie.Controllers
{
    [Produces("application/json")]
    [Route("api/Ratings")]
    public class RatingsController : Controller
    {
        private MVCMovieContext _context;

        public RatingsController(MVCMovieContext context)
        {
            _context = context;
        }

        // GET: api/Ratings
        [HttpGet]
        public IEnumerable<Review> GetRating()
        {
            return _context.Rating;
        }

        // GET: api/Ratings/5
        [HttpGet("{id}", Name = "GetRating")]
        public IActionResult GetRating([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Review rating = _context.Rating.Single(m => m.Id == id);

            if (rating == null)
            {
                return HttpNotFound();
            }

            return Ok(rating);
        }

// PUT: api/Ratings/5
[HttpPut("{id}")]
public IActionResult PutRating(int id, [FromBody] Review rating)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != rating.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Ratings
        [HttpPost]
        public IActionResult PostRating([FromBody] Review rating)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Rating.Add(rating);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RatingExists(rating.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetRating", new { id = rating.Id }, rating);
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRating(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Review rating = _context.Rating.Single(m => m.Id == id);
            if (rating == null)
            {
                return HttpNotFound();
            }

            _context.Rating.Remove(rating);
            _context.SaveChanges();

            return Ok(rating);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RatingExists(int id)
        {
            return _context.Rating.Count(e => e.Id == id) > 0;
        }
    }
}