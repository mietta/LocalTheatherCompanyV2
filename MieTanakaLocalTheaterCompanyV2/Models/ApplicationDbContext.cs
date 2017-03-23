using MieTanakaLocalTheaterCompanyV2.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace MieTanakaLocalTheaterCompanyV2.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
 
        public ApplicationDbContext(): base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations.Add(new BlogTypeConfiguration());
            modelBuilder.Configurations.Add(new CommentTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public class BlogTypeConfiguration : EntityTypeConfiguration<Blog>
        {
            public BlogTypeConfiguration()
            {//one-to-many
             //               HasOptional(b => b.Category)
             //                                         .WithMany(c => c.Blogs).HasForeignKey(b => b.CategoryId);
                HasOptional(b => b.ApplicationUser)
                                   .WithMany(u => u.Blogs).HasForeignKey(b => b.Id);
                //HasMany(b => b.Comments).WithRequired(c => c.Blog)
                //.HasForeignKey(c => c.BlogId).WillCascadeOnDelete(true);
            }
        }

        public class CommentTypeConfiguration : EntityTypeConfiguration<Comment>
        {
            public CommentTypeConfiguration()
            {//one-to-many
             //               HasOptional<Blog>(C => C.Blog)
             //                                         .WithMany(b => b.Comments).HasForeignKey(c => c.BlogId);
                HasOptional(b => b.ApplicationUser)
                                   .WithMany(u => u.Comments).HasForeignKey(c => c.Id);
                // HasMany(b => b.Comments).WithRequired(c => c.Blog)
                // .HasForeignKey(c => c.BlogId).WillCascadeOnDelete(true);
            }
        }

        /*        protected override void OnModelCreating(DbModelBuilder modelBuilder)
                {
                    //configure default schema
                    //            modelBuilder.HasDefaultSchema("Admin");
                    //Map entity to table
                    //         modelBuilder.Entity<Category>().ToTable("Category");
                    //         modelBuilder.Entity<Blog>().ToTable("Blogs");
                    //        modelBuilder.Entity<Comment>().ToTable("Comments");
        //                       modelBuilder.Entity<Category>().HasMany(c => c.Blogs).WithOptional(b => b.Category)
        //                                  .HasForeignKey(b => b.CategoryId).WillCascadeOnDelete(false);
        //                       modelBuilder.Entity<Blog>().HasMany(b => b.Comments).WithRequired(c => c.Blog)
        //                                  .HasForeignKey(c => c.BlogId).WillCascadeOnDelete(true);
        //                       modelBuilder.Entity<ApplicationUser>().HasMany(ApplicationUser => ApplicationUser.Blogs).WithOptional(b => b.ApplicationUser)
        //                                  .HasForeignKey(b => b.Id).WillCascadeOnDelete(false);
        //                       modelBuilder.Entity<ApplicationUser>().HasMany(ApplicationUser => ApplicationUser.Comments).WithOptional(c => c.ApplicationUser)
        //                                  .HasForeignKey(c => c.Id).WillCascadeOnDelete(false);


                                modelBuilder.Entity<Blog>().HasRequired<Category>(b => b.Category)
                                         .WithMany(c => c.Blogs).HasForeignKey(b => b.CategoryId).WillCascadeOnDelete(false);

                                modelBuilder.Entity<Blog>().HasRequired<ApplicationUser>(b => b.ApplicationUser)
                                             .WithMany(u => u.Blogs).HasForeignKey(b => b.Id).WillCascadeOnDelete(false);

                                modelBuilder.Entity<Comment>().HasRequired<Blog>(c => c.Blog)
                                            .WithMany(b => b.Comments).HasForeignKey(c => c.BlogId).WillCascadeOnDelete(true);

                                modelBuilder.Entity<Comment>().HasRequired<ApplicationUser>(c => c.ApplicationUser)
                                         .WithMany(u => u.Comments).HasForeignKey(c => c.Id).WillCascadeOnDelete(false);

                }      
                /*
                */



    }

    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>{

        protected override void Seed(ApplicationDbContext context)
        {

            //Initialize Iendity(context);
            if (!context.Users.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var userStore = new UserStore<ApplicationUser>(context);
                //_________________________________________________________________________
                //populating the role table with the data in side of RoleName class using RoleManager.
                //Role Name class does not need to be created if simplly add "Administrator", "Dog_Owner", "Dog_Walker" string
                if (!roleManager.RoleExists(RoleName.ROLE_ADMINSTRATOR))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleName.ROLE_ADMINSTRATOR));
                }
                if (!roleManager.RoleExists(RoleName.ROLE_USER))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleName.ROLE_USER));
                }
                if (!roleManager.RoleExists(RoleName.ROLE_STAFF))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleName.ROLE_STAFF));
                }

                //pupulating category table
                List<Category> categories = new List<Category>();

                categories.Add(new Category { CategoryName = "Play" });
                categories.Add(new Category { CategoryName = "Comedy" });
                categories.Add(new Category { CategoryName = "Gig" });
                foreach (Category ctgy in categories) context.Categories.Add(ctgy);
                context.SaveChanges();

                string userName1 = "admin@admin2.com";
                string password1 = "Password#2";
                // var passworddHash = new PasswordHasher(); if migration enable methods this code needs used
                //password = passwordHash.HashPassword(password);
                // Create Admin user and role

                var user = userManager.FindByName(userName1);
                if (user == null)
                {
                    var newUser = new ApplicationUser()
                    {
                        Forename = "Administrator",
                        Surname = "Admin",
                        Street = "190 Catheral Street",
                        Town = "Glasgow",
                        Postcode = "2G1 3DF",
                        PhoneNumber = "0141  43455",
                        UserName = userName1,
                        Email = userName1,
                        EmailConfirmed = true,
                    };
                    var objBlog1 = new Blog()
                    { BlogDate = Convert.ToDateTime("27/10/2016"),
                        BlogTitle = "The Full Monty Overview",
                        BlogContent = "The classic film about six out of work steel workers with nothing to lose took the world by storm! Based on his smash hit film and adapted for the stage by Oscar-winning writer Simon Beaufoy, this hilarious and heartfelt production stars Gary Lucy, Andrew Dunn, Louis Emerick, Chris Fountain, Anthony Lewis, Kai Owen and a cast of fourteen and is directed by Jack Ryder. Not only has the play been getting standing ovations every night, but it also won the prestigious UK Theatre Award for best touring production. Featuring great songs by Donna Summer, Hot Chocolate and Tom Jones you really should...drop absolutely everything and book today!",
                        BlogApproved =true,
                        CategoryId = 1, Id = newUser.Id };
                    var objCommnet1 = new Comment()
                    { CommentedDate = Convert.ToDateTime("15/01/2017"),
                        CommentTitle = "good show",
                        CommentBody = " must go, but there was not car park near the theater give good 40 min before arriving the theater",
                        BlogId = 1, Id = newUser.Id };
                    context.Blogs.Add(objBlog1);
                    context.Comments.Add(objCommnet1);
                    //userManager.Create(newUser, password);
                    //userManager.AddToRole(newUser,Role);
                    userManager.Create(newUser, password1);
                    //userManager.SetLockourtEnabled(newUser.Id,false)
                    userManager.AddToRole(newUser.Id, RoleName.ROLE_ADMINSTRATOR);
                }

                var newUser2 = new ApplicationUser()
                {
                    Forename = "Mie",
                    Surname = "Tanaka",
                    Street = "1 Glasgow Street",
                    Town = "Glasgow",
                    Postcode = "G1 5AA",
                    PhoneNumber = "014112345",
                    UserName = "mietta@gmail.com",
                    Email = "mietta@gmail.com",
                    EmailConfirmed = true,
                };

                var objBlog2 = new Blog() { BlogDate = Convert.ToDateTime("20/11/2016"),
                    BlogTitle = "Frankie Boyle and Friends Overview",
                    BlogContent = "Fresh from hosting his critically-acclaimed American Autopsy for the BBC, join Frankie Boyle and some of his favourite comics for a run of shows in his native Glasgow as part of the Glasgow Live International Comedy Festival. Expect ‘black-hearted brilliance’ The Guardian from one of UK comedy’s fiercest talents, as well as a selection of the brightest and best comedians currently working on the circuit. Thursday night’s show proceeds go to the charity Help Refugees UK. Latecomers may not be admitted. There will be no re-admittance during the second half of the show.",
                    BlogApproved = true,
                    CategoryId = 2 ,Id=newUser2.Id};
                var objBlog3 = new Blog()
                {
                    BlogDate = Convert.ToDateTime("20/02/2017"),
                    BlogTitle = "Roy Wood & his Band",
                    BlogContent = "Featuring Roy Wood & his Rock ‘N Roll band performing all his classic hits, I Can Hear The Grass Grow, Flowers In The Rain, Blackberry Way, California Man, See My Baby Jive, I Wish It Could Be Christmas Everyday….and much more.Born in Birmingham, His first instrument was drums, which is the only instrument he has ever had any tuition on! Influenced by Hank Marvin's sound, he took up playing guitar, and formed a group called The Falcons at the age of forteen In 1970, Roy teamed up with fellow Birmingham songwriter Jeff Lynne, who joined The Move for their final two albums. The final single recorded by The Move during this period was California Man. Roy had an ambition over a number of years, to form a classically based band featuring live strings instead of the conventional guitar line up. Together with Jeff, they formed The Electric Light Orchestra. Support Act: Nik Lowe, an acoustic soul/pop/rock/blues/crooner like no other. It’s hard to put Nik Lowe into a box labelled with a particular genre of music becase Nik's song-writing style covers most genres. One minute his music is rocky and raw, the next smooth and schmaltzy, then on to pure retro-pop - and all with a voice to match. Don’t miss this exclusive show!",
                    BlogApproved = false,
                    CategoryId = 3,
                    Id = newUser2.Id
                };

                var objComment2 = new Comment()
                 { CommentedDate = Convert.ToDateTime("26/01/2017"),
                     CommentTitle = "so so",
                     CommentBody = " it was boring i wish i stayed at home, i should have booked comedy show instead",
                     BlogId = 3, Id = newUser2.Id };
                
                var objComment = new Comment()
                { CommentedDate = Convert.ToDateTime("27/01/2017"),
                    CommentTitle = "really goog",
                    CommentBody = " I had a  black joke giggles the whole time, he could use less swearing words",
                    BlogId = 2, Id = newUser2.Id };
                context.Blogs.Add(objBlog2);
                context.Blogs.Add(objBlog3);
                context.Comments.Add(objComment2);
                userManager.Create(newUser2, "Password#2");
                userManager.AddToRole(newUser2.Id, RoleName.ROLE_STAFF);
            }

            base.Seed(context);
            context.SaveChanges();
        }
    }
}