using NewPizzeria.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewPizzeria.Controllers
{
    public class HomeController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult BackOffice()
        {
            ViewBag.Title = "Back Office";

            return View();
        }

        [HttpGet]
        public ActionResult CalcolaIncasso(DateTime data)
        {
            // Interroga il database per ottenere tutti gli ordini finalizzati per la data specificata
            var ordini = db.Ordini
                .Where(o => DbFunctions.TruncateTime(o.OrderDate) == data.Date && o.Status == "Finalizzato")
                .ToList();

            // Calcola l'importo totale delle vendite per la data specificata
            decimal incassoTotale = 0;
            foreach (var ordine in ordini)
            {
                foreach (var dettaglio in ordine.DettaglioOrdini)
                {
                    // Assicurati che il prezzo non sia null prima di calcolare l'importo totale
                    if (dettaglio.Prodotti.Price != null)
                    {
                        // Moltiplica il prezzo per la quantità e aggiungi all'incasso totale
                        incassoTotale += (decimal)(dettaglio.Quantity * dettaglio.Prodotti.Price);
                    }
                }
            }

            // Ritorna l'importo totale come una stringa
            return Content(incassoTotale.ToString());
        }


    }
}
