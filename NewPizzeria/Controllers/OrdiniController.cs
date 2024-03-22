using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NewPizzeria.Models;

namespace NewPizzeria.Controllers
{
    [Authorize]
    public class OrdiniController : Controller
    {

        private ModelDbContext db = new ModelDbContext();

        // GET: Ordini
        public ActionResult Index()
        {
            var ordini = db.Ordini
                            .Include(o => o.Prodotti)
                            .Include(o => o.Utenti)
                            .OrderBy(o => o.OrderDate)
                            .ToList();

            return View(ordini);
        }

        public ActionResult OrdiniFinalizzati()
        {
            var ordini = db.Ordini
                            .Include(o => o.Prodotti)
                            .Include(o => o.Utenti)
                            .Where(o => o.Status == "Finalizzato")
                            .OrderBy(o => o.OrderDate)
                            .ToList();

            return View(ordini);
        }


        public ActionResult OrdiniEvasi()
        {
            var ordini = db.Ordini.Include(o => o.Prodotti).Include(o => o.Utenti).Where(o => o.Status == "Evaso");
            return View(ordini.ToList());
        }
        // GET: Ordini/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // GET: Ordini/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Prodotti, "ProductId", "Name");
            ViewBag.UserId = new SelectList(db.Utenti, "UserId", "Username");
            return View();
        }

        // POST: Ordini/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,UserId,OrderDate,ShippingAddress,Notes,Status,ProductId")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Ordini.Add(ordini);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Prodotti, "ProductId", "Name", ordini.ProductId);
            ViewBag.UserId = new SelectList(db.Utenti, "UserId", "Username", ordini.UserId);
            return View(ordini);
        }

        // GET: Ordini/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Prodotti, "ProductId", "Name", ordini.ProductId);
            ViewBag.UserId = new SelectList(db.Utenti, "UserId", "Username", ordini.UserId);
            return View(ordini);
        }

        // POST: Ordini/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,UserId,OrderDate,ShippingAddress,Notes,Status,ProductId")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordini).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Prodotti, "ProductId", "Name", ordini.ProductId);
            ViewBag.UserId = new SelectList(db.Utenti, "UserId", "Username", ordini.UserId);
            return View(ordini);
        }

        // GET: Ordini/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordini ordini = db.Ordini.Include(o => o.DettaglioOrdini).SingleOrDefault(o => o.OrderId == id);
            if (ordini != null)
            {
                db.DettaglioOrdini.RemoveRange(ordini.DettaglioOrdini);
                db.Ordini.Remove(ordini);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            // Trova il prodotto con l'ID specificato nel database
            Prodotti product = db.Prodotti.Find(id);

            if (product == null)
            {
                // Se il prodotto non esiste, restituisci un errore
                return HttpNotFound();
            }

            // Ottieni l'ID dell'utente corrente
            string currentUsername = User.Identity.Name;
            Utenti currentUser = db.Utenti.FirstOrDefault(u => u.Username == currentUsername);

            if (currentUser == null)
            {
                // Gestisci l'errore qui se l'utente non è trovato
                return RedirectToAction("Login", "Utenti");
            }

            int userId = currentUser.UserId;

            // Trova l'ordine dell'utente corrente
            Ordini order = db.Ordini.FirstOrDefault(o => o.UserId == userId && o.Status == "In Cart");

            if (order == null)
            {
                // Se non esiste un ordine in carrello per l'utente corrente, crea un nuovo ordine
                order = new Ordini
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Status = "In Cart"
                };

                // Aggiungi l'ordine al database
                db.Ordini.Add(order);
                db.SaveChanges();
            }

            // Verifica se il prodotto è già presente nel carrello
            DettaglioOrdini orderItem = order.DettaglioOrdini.FirstOrDefault(item => item.ProductId == id);

            if (orderItem == null)
            {
                // Se il prodotto non è presente nel carrello, aggiungilo
                order.DettaglioOrdini.Add(new DettaglioOrdini { ProductId = product.ProductId, Quantity = quantity });
            }
            else
            {
                // Se il prodotto è già presente nel carrello, aggiorna la quantità
                orderItem.Quantity += quantity;
            }

            // Salva le modifiche nel database
            db.SaveChanges();

            // Ritorna una risposta JSON per indicare che l'operazione è riuscita
            return Json(new { success = true });
        }


        public ActionResult Cart()
        {
            // Ottieni l'utente corrente
            string currentUsername = User.Identity.Name;
            Utenti currentUser = db.Utenti.FirstOrDefault(u => u.Username == currentUsername);

            if (currentUser == null)
            {
                // Gestisci l'errore se l'utente non è trovato
                return RedirectToAction("Login", "Utenti");
            }

            // Ottieni l'ID dell'utente corrente
            int userId = currentUser.UserId;

            // Trova l'ordine corrente dell'utente con stato "In Carrello"
            Ordini order = db.Ordini.Include(o => o.DettaglioOrdini.Select(d => d.Prodotti))
                                     .FirstOrDefault(o => o.UserId == userId && o.Status == "In Cart");

            if (order == null)
            {
                // Se non esiste un ordine in carrello, restituisci una vista vuota
                return View(new List<DettaglioOrdini>());
            }

            // Calcola la media del tempo di consegna dei prodotti nel carrello
            int totalDeliveryTime = 0;
            int totalProducts = 0;

            foreach (var item in order.DettaglioOrdini)
            {
                if (item.Prodotti.DeliveyTime.HasValue)
                {
                    totalDeliveryTime += item.Prodotti.DeliveyTime.Value;
                    totalProducts++;
                }
            }

            int averageDeliveryTime = totalProducts > 0 ? totalDeliveryTime / totalProducts : 0;

            // Restituisci la vista con i dettagli dell'ordine corrente e la media del tempo di consegna
            ViewBag.AverageDeliveryTime = averageDeliveryTime;
            return View(order.DettaglioOrdini.ToList());
        }


        [HttpPost]
        public ActionResult ConfirmOrder(string shippingAddress, string notes)
        {
            // Ottieni l'utente corrente
            string currentUsername = User.Identity.Name;
            Utenti currentUser = db.Utenti.FirstOrDefault(u => u.Username == currentUsername);

            if (currentUser == null)
            {
                // Gestisci l'errore se l'utente non è trovato
                return RedirectToAction("Login", "Utenti");
            }

            // Ottieni l'ID dell'utente corrente
            int userId = currentUser.UserId;

            // Trova l'ordine corrente dell'utente con stato "In Carrello"
            Ordini order = db.Ordini.FirstOrDefault(o => o.UserId == userId && o.Status == "In Cart");

            if (order == null)
            {
                // Se non esiste un ordine in carrello, restituisci una vista vuota
                return RedirectToAction("Cart");
            }

            // Aggiorna lo stato dell'ordine a "Finalizzato"
            order.Status = "Finalizzato";
            // Aggiorna l'indirizzo di spedizione e le note
            order.ShippingAddress = shippingAddress;
            order.Notes = notes;

            // Salva le modifiche nel database
            db.SaveChanges();

            // Ritorna una vista di conferma dell'ordine o un'altra azione desiderata
            return RedirectToAction("OrderConfirmed");
        }

        public ActionResult OrderConfirmed()
        {
            return View();
        }

        public int GetCartItemCount()
        {
            // Ottieni l'utente corrente
            string currentUsername = User.Identity.Name;
            Utenti currentUser = db.Utenti.FirstOrDefault(u => u.Username == currentUsername);

            if (currentUser == null)
            {
                // Gestisci l'errore se l'utente non è trovato
                return 0;
            }

            // Ottieni l'ID dell'utente corrente
            int userId = currentUser.UserId;

            // Trova il numero di prodotti nel carrello per l'utente corrente
            var orders = db.Ordini.Include(o => o.DettaglioOrdini)
                                  .Where(o => o.UserId == userId && o.Status == "In Cart")
                                  .SelectMany(o => o.DettaglioOrdini);

            int cartItemCount = (int)(orders.Any() ? orders.Sum(d => d.Quantity) : 0);

            return cartItemCount;
        }


        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Ottieni l'utente corrente
            string currentUsername = User.Identity.Name;
            Utenti currentUser = db.Utenti.FirstOrDefault(u => u.Username == currentUsername);

            if (currentUser == null)
            {
                // Gestisci l'errore se l'utente non è trovato
                return RedirectToAction("Login", "Utenti");
            }

            // Ottieni l'ID dell'utente corrente
            int userId = currentUser.UserId;

            // Trova l'ordine corrente dell'utente con stato "In Carrello"
            Ordini order = db.Ordini.Include(o => o.DettaglioOrdini)
                                     .FirstOrDefault(o => o.UserId == userId && o.Status == "In Cart");

            if (order == null)
            {
                // Se non esiste un ordine in carrello, restituisci una vista vuota
                return RedirectToAction("Cart");
            }

            // Trova il dettaglio dell'ordine del prodotto da rimuovere
            DettaglioOrdini orderItem = order.DettaglioOrdini.FirstOrDefault(item => item.ProductId == id);

            if (orderItem != null)
            {
                // Rimuovi il prodotto dall'ordine
                order.DettaglioOrdini.Remove(orderItem);
                db.DettaglioOrdini.Remove(orderItem);
                // Salva le modifiche nel database
                db.SaveChanges();
            }

            // Ritorna una risposta JSON per indicare che l'operazione è riuscita
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Evaso(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Trova l'ordine dal database
            Ordini order = db.Ordini.Find(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Verifica se l'utente corrente è autorizzato a modificare lo stato dell'ordine
            if (!User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Assicurati che l'ordine sia nello stato "Finalizzato"
            if (order.Status != "Finalizzato")
            {
                // Se l'ordine non è nello stato corretto, esegui un reindirizzamento o restituisci un messaggio di errore
                return RedirectToAction("Index");
            }

            // Cambia lo stato dell'ordine da "Finalizzato" a "Evaso"
            order.Status = "Evaso";

            // Salva le modifiche nel database
            db.SaveChanges();

            // Ritorna alla vista Index o ad un'altra azione desiderata
            return RedirectToAction("Evaso");
        }








    }
}
