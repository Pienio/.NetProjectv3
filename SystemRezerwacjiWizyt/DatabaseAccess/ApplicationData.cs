using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    class ApplicationData : DbContext, ITransactionalApplicationData
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<ProfileRequest> Requests { get; set; }

        public bool IsTransactionRunning { get; private set; } = false;
        public bool CommitUnfinishedTransaction { get; set; } = true;

        public bool IsDisposed { get; set; } = false;
        public bool ToCommit { get; set; } = true;

        public ApplicationData(bool runTransaction) : base("SysRejWiz")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationData>());

            Configuration.ProxyCreationEnabled = false;
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            objectContext.ContextOptions.LazyLoadingEnabled = false;
            var x = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            if (runTransaction)
            {
                Database.BeginTransaction();
                IsTransactionRunning = true;
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Visit>().HasRequired(t=>t.Patient).WithRequiredPrincipal().WillCascadeOnDelete(false);//.WithRequired().WillCascadeOnDelete(false);
            //modelBuilder.Entity<Visit>().HasRequired(p => p.Patient).WithOptional(p => p.Visits);
            modelBuilder.Entity<Doctor>().HasMany(p => p.Visits).WithRequired().WillCascadeOnDelete(false);
            modelBuilder.Entity<Patient>().HasMany(p => p.Visits).WithRequired().WillCascadeOnDelete(false);
        }

        public void Fill()
        {
            Specialization[] specs = {
            new Specialization("Reumatolog"),
            new Specialization("Kardiolog"),
            new Specialization("Neurolog"),
            new Specialization("Urolog"),
            new Specialization("Okulista"),
            new Specialization("Psychiatra"),
            new Specialization("Ginekolog"),
            new Specialization("Pediatra")};

            if (!Specializations.Any())
            {
                Specializations.AddRange(specs);
                this.SaveChanges();
            }
            if (!Users.Any())
            {
                string[] names = { "Kuba", "Jan", "Łukasz", "Adrian", "Bartosz", "Marek", "Filip", "Bartłomiej" };
                string[] surnames = { "Soczkowski", "Berwid", "Okular", "Michałowski", "Skała", "Mikowski", "Wasiłkowski", "Normowski" };
                string[] pesels = { "09586749381", "19683750923", "94860285691", "58672349682", "38596827364", "58476923857", "88975643287", "29384795618" };
                for (int i = 0; i < 8; i++)
                {
                    Doctor ne = new Doctor();
                    
                    ne.ProfileAccepted = true;
                    ne.User = new User() { Name = new PersonName() };
                    ne.User.Mail = "sysrejwiz@gmail.com";
                    ne.User.Name.Name = names[i];
                    ne.User.Name.Surname = surnames[i];
                    ne.User.PESEL = pesels[i];
                    ne.User.Password = "96e79218965eb72c92a549dd5a330112";
                    ne.User.Kind = DocOrPat.Doctor;
                    ne.MondayWorkingTime = new WorkingTime();
                    ne.MondayWorkingTime.Start = 8 + i / 2;
                    ne.MondayWorkingTime.End = 12 + i / 2;
                    ne.TuesdayWorkingTime = new WorkingTime();
                    ne.TuesdayWorkingTime.Start = 8 + i / 2;
                    ne.TuesdayWorkingTime.End = 12 + i / 2;
                    ne.WednesdayWorkingTime = new WorkingTime();
                    ne.WednesdayWorkingTime.Start = 8 + i / 2;
                    ne.WednesdayWorkingTime.End = 12 + i / 2;
                    ne.ThursdayWorkingTime = new WorkingTime();
                    ne.ThursdayWorkingTime.Start = 8 + i / 2;
                    ne.ThursdayWorkingTime.End = 12 + i / 2;
                    ne.FridayWorkingTime = new WorkingTime();
                    ne.FridayWorkingTime.Start = 8 + i / 2;
                    ne.FridayWorkingTime.End = 12 + i / 2;
                    ne.Specialization.Add(specs[i]);
                    Doctors.Add(ne);
                }
                this.SaveChanges();
            }

        }
        
        public void BeginTransaction()
        {
            if (IsTransactionRunning)
            {
                if (CommitUnfinishedTransaction)
                    Commit();
                else
                    Rollback();
            }
            Database.BeginTransaction();
            IsTransactionRunning = true;
        }

        public void Commit()
        {
            if (IsTransactionRunning)
            {
                if (!ToCommit) return;
                try
                {
                    SaveChanges();
                    IsTransactionRunning = false;
                    Database.CurrentTransaction.Commit();
                }
                catch
                {
                    Database.CurrentTransaction.Rollback();
                    throw;
                }
            }
            else
                throw new InvalidOperationException("Brak aktywnej transakcji.");
        }

        public void SaveChangesOn()
        {
            this.SaveChanges();
        }

        public void Rollback()
        {
            if (IsTransactionRunning)
            {
                Database.CurrentTransaction.Rollback();
                IsTransactionRunning = false;
            }
            else
                throw new InvalidOperationException("Brak aktywnej transakcji.");
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                if (disposing)
                    throw new InvalidOperationException("Nie można zwolnić DBContext ponownie.");
                return;
            }
            IsDisposed = true;

            if (IsTransactionRunning)
            {
                if (CommitUnfinishedTransaction)
                    Commit();
                else
                    Rollback();
            }

            base.Dispose(disposing);
        }
    }
}
