using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExploreBookings.Models;
using ExploreBookings.Models.Logic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using PayFast;
using PayFast.AspNet;

namespace ExploreBookings.Controllers
{
    public class RoomBookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RoomBookings
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
            var roomBookings = db.RoomBookings.Include(r => r.Room);
            return View(roomBookings.ToList().Where(x => x.ManagerEmail == userName));
        }
        public ActionResult ChckIn()
        {
            var userName = User.Identity.GetUserName();
            var roomBookings = db.RoomBookings.Include(r => r.Room);
            return View(roomBookings.ToList().Where(x=>x.ManagerEmail == userName));
        }
        public ActionResult CheckInn(int? id)
        {
            RoomBooking bookingRoom = db.RoomBookings.Find(id);
            var guestName = db.guests.ToList().Where(p => p.Email == User.Identity.GetUserName()).Select(p => p.FullName).FirstOrDefault();
            if (bookingRoom.Status == "Checked Out")
            {
                TempData["AlertMessage"] = "Cannot check in a person whohas already been checked out";
                return RedirectToAction("ChckIn");
            }
            else
            if (bookingRoom.Status == "Checked In")
            {
                TempData["AlertMessage"] = "Cannot check in " + guestName + " twice";
                return RedirectToAction("ChckIn");
            }
            else
            if (id != null)
            {
                bookingRoom.Status = "Checked In";
                db.Entry(bookingRoom).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = guestName + " Has been Successfully checked in";
                return RedirectToAction("ChckIn");
            }
            return View();
        }
        public ActionResult CheckOut(int? id)
        {
            RoomBooking bookingRoom = db.RoomBookings.Find(id);
            var guestName = db.guests.ToList().Where(p => p.Email == User.Identity.GetUserName()).Select(p => p.FullName).FirstOrDefault();

            if (bookingRoom.Status == "Not yet Checked In!!")
            {
                TempData["AlertMessage"] = "Cannot check out a person who has not been checked in";
                return RedirectToAction("ChckIn");
            }
            else if (bookingRoom.Status == "Checked Out")
            {
                TempData["AlertMessage"] = "Cannot check out " + guestName + " twice";
                return RedirectToAction("ChckIn");
            }
            else if (id != null)
            {
                bookingRoom.Status = "Checked Out";
                db.Entry(bookingRoom).State = EntityState.Modified;
                db.SaveChanges();

                var q = db.Rooms.Where(p => p.RoomId == bookingRoom.RoomId).Select(p => p.roomtypeId).FirstOrDefault();
                var rty = db.RoomTypes.Where(p => p.RoomtypeId == q).FirstOrDefault();
                rty.RoomAvailable++;
                db.Entry(rty).State = EntityState.Modified;
                db.SaveChanges();

                TempData["AlertMessage"] = guestName + " Successfully checked Out";
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult MyBookings()
        {
            var userName = User.Identity.GetUserName();
            var roomBookings = db.RoomBookings.Include(r => r.Room);
            return View(roomBookings.ToList().Where(x => x.GuestEmail == userName));
        }
        // GET: RoomBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomBooking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }
        public ActionResult ConfirmBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomBooking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }

        // GET: RoomBookings/Create
        public ActionResult Create(int? id)
        {
            //ViewBag.Id = id;
            Session["RoomId"] = id;
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription");
            return View();
        }

        // POST: RoomBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,RoomPrice,CheckInDate,CheckOutDate,NumberOfDays,NumberOfPeople")] RoomBooking roomBooking)
        {
            var userName = User.Identity.GetUserName();
            roomBooking.RoomId = int.Parse(Session["RoomId"].ToString());

            var recordss = db.RoomBookings.Where(x => x.RoomId == roomBooking.RoomId).Select(x => x.CheckOutDate).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (roomBooking.CheckInDate >= recordss)
                {
                    if (BusinessLogic.dateLessChecker(roomBooking) == false)
                    {
                        if (BusinessLogic.dateLessOutChecker(roomBooking) == false)
                        {
                         
                            roomBooking.GuestEmail = userName;
                            roomBooking.ManagerEmail = BusinessLogic.GetHotelManagerEmail(roomBooking.RoomId);
                            roomBooking.RoomPrice = BusinessLogic.GetRoomPrice(roomBooking.RoomId);
                            roomBooking.RoomType = BusinessLogic.GetRoomType(roomBooking.RoomId);
                            roomBooking.NumberOfDays = BusinessLogic.GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);
                            roomBooking.Status = "Not yet Checked In!!";
                            roomBooking.Total = BusinessLogic.calcTotalRoomCost(roomBooking);
                            roomBooking.HotelAddress = BusinessLogic.GetHotelAddress(roomBooking.RoomId);

                            db.RoomBookings.Add(roomBooking);
                            db.SaveChanges();
                            Session["bookID"] = roomBooking.RoomBookingId;
                            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                            return RedirectToAction("ConfirmBooking", new { id = roomBooking.RoomBookingId });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Check in date cannot be greater than checkout date!!");
                            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                            return View(roomBooking);
                        }

                    }
                    else
                    {

                        ModelState.AddModelError("", "Cannot book for a date that has passed!!");
                        ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                        return View(roomBooking);
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"Room already booked!! Please Choose date after {recordss}");
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                    return View(roomBooking);
                }
              
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }

        // GET: RoomBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomBooking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }

        // POST: RoomBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,RoomPrice,CheckInDate,CheckOutDate,NumberOfDays,NumberOfPeople")] RoomBooking roomBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }

        // GET: RoomBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomBooking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }

        // POST: RoomBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomBooking roomBooking = db.RoomBookings.Find(id);
            db.RoomBookings.Remove(roomBooking);
            db.SaveChanges();
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
        public RoomBookingsController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }
        //Payment
        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        #region Constructor

        //public ApprovedOwnersController()
        //{

        //}

        #endregion Constructor

        #region Methods



        public ActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            recurringRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";

            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "drnendwandwe@gmail.com";

            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult OnceOff()
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            int ReservationID = int.Parse(Session["bookID"].ToString());
            RoomBooking roomBooking = new RoomBooking();
            roomBooking = db.RoomBookings.Find(ReservationID);
            var userName = User.Identity.GetUserName();
            var guest = db.guests.Where(x => x.Email == userName).FirstOrDefault();

            var attachments = new List<Attachment>();
            attachments.Add(new Attachment(new MemoryStream(GeneratePDF(ReservationID)), "Reservation Receipt", "application/pdf"));


            var mailTo = new List<MailAddress>();
            mailTo.Add(new MailAddress(User.Identity.GetUserName(), guest.FullName));
            var body = $"Hello {guest.FullName}, please see attached receipt for the recent reservation you made. <br/>Make sure you bring along your receipt when you check in for your room.<br/>";
            ExploreBookings.Models.EmailService emailService = new ExploreBookings.Models.EmailService();
            emailService.SendEmail(new EmailContent()
            {
                mailTo = mailTo,
                mailCc = new List<MailAddress>(),
                mailSubject = "Application Statement | Ref No.:" + roomBooking.RoomBookingId,
                mailBody = body,
                mailFooter = "<br/> Many Thanks, <br/> <b>Explorer</b>",
                mailPriority = MailPriority.High,
                mailAttachments = attachments

            });
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details

            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            //onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = Convert.ToDouble(roomBooking.Total);
            onceOffRequest.item_name = "Room Booking payment";
            onceOffRequest.item_description = "Some details about the once off payment";

            BusinessLogic.UpdateRoomsAvailable(roomBooking.RoomId);
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            return Redirect(redirectUrl);
        }

        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Error()
        {
            return View();
        }

        #endregion Methods
        public byte[] GeneratePDF(int BookingID)
        {
            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document(PageSize.A5, 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            RoomBooking roomBooking = new RoomBooking();
            roomBooking = db.RoomBookings.Find(BookingID);
            var userName = User.Identity.GetUserName();
            var guest = db.guests.Where(x => x.Email == userName).FirstOrDefault();

            iTextSharp.text.Font font_heading_3 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font_body = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.BaseColor.BLUE);

            // Create the heading paragraph with the headig font
            PdfPTable table1 = new PdfPTable(1);
            PdfPTable table2 = new PdfPTable(5);
            PdfPTable table3 = new PdfPTable(1);

            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -6f;
            // Remove table cell
            table1.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table3.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            table1.WidthPercentage = 80;
            table1.SetWidths(new float[] { 100 });
            table2.WidthPercentage = 80;
            table3.SetWidths(new float[] { 100 });
            table3.WidthPercentage = 80;

            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Colspan = 3;
            table1.AddCell("\n");
            table1.AddCell(cell);
            table1.AddCell("\n\n");
            table1.AddCell(
                "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t" +
                "Homelink \n" +
                "Email :homelink.grp18@gmail.com" + "\n" +
                "\n" + "\n");
            table1.AddCell("------------Your Details--------------!");

            table1.AddCell("Full Name : \t" + guest.FullName);
            table1.AddCell("Last Name : \t" + guest.LastName);
            table1.AddCell("Email : \t" + guest.Email);
            table1.AddCell("Phone Number : \t" + guest.Phone);
            table1.AddCell("Gender : \t" + guest.Gender);
            table1.AddCell("Address : \t" + guest.Address);

            table1.AddCell("\n------------Booking details--------------!\n");

            table1.AddCell("Booking # : \t" + roomBooking.RoomBookingId);
            table1.AddCell("Room Type : \t" + roomBooking.RoomType);
            table1.AddCell("Room Price Per Night: \t" + roomBooking.RoomPrice.ToString("C"));
            table1.AddCell("Arrival date : \t" + roomBooking.CheckInDate);
            table1.AddCell("Departure date : \t" + roomBooking.CheckOutDate);
            table1.AddCell("Number Of days : \t" + roomBooking.NumberOfDays);
            table1.AddCell("Number Of People : \t" + roomBooking.NumberOfPeople);
            table1.AddCell("Total Room Cost: \t" + roomBooking.Total.ToString("C"));

            table1.AddCell("\n");

            table3.AddCell("------------Looking forward to hear from you soon--------------!");

            //////Intergrate information into 1 document
            //var qrCode = iTextSharp.text.Image.GetInstance(reservation.QrCodeImage);
            //qrCode.ScaleToFit(200, 200);
            table1.AddCell(cell);
            document.Add(table1);
            //document.Add(qrCode);
            document.Add(table3);
            document.Close();

            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return bytes;
        }
    }
}
