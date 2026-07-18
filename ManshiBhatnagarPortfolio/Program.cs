using ManshiBhatnagarPortfolio.Data;
using ManshiBhatnagarPortfolio.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.SlidingExpiration = true;
    });
var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//    db.Database.Migrate();
//}
//var uploadPath = Path.Combine(builder.Environment.WebRootPath, "uploads");
//if (!Directory.Exists(uploadPath))
//{
//    Directory.CreateDirectory(uploadPath);
//}
var webRootPath = builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
var uploadPath = Path.Combine(webRootPath, "uploads");

if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Seed Articles
    if (!context.Articles.Any())
    {
        context.Articles.AddRange(new List<Article>
        {
            new Article
            {
                Title = "Advancements in Computer Vision for Smart Energy Systems",
                Slug = "advancements-in-computer-vision-for-smart-energy-systems",
                Content = "Computer Vision is revolutionizing how we monitor and optimize energy grids. By analyzing thermal imaging, satellite footage, and camera sensors, we can detect anomalies in solar panels, inspect wind turbines, and prevent outages in electrical grids before they occur. This article explores recent state-of-the-art CNN and transformer models applied to energy system telemetry.",
                ImageUrl = "/uploads/cv-energy.png",
                Author = "Dr. Mansi Bhatnagar",
                Category = "Computer Vision",
                ReadTime = 5,
                CreatedDate = DateTime.Now.AddDays(-10)
            },
            new Article
            {
                Title = "Deep Learning Architectures for Smart Grid Optimization",
                Slug = "deep-learning-architectures-for-smart-grid-optimization",
                Content = "Managing modern power grids requires handling massive, multi-dimensional time series data. Deep learning frameworks, particularly LSTM and Graph Neural Networks (GNNs), offer powerful tools for forecasting demand, modeling grid topology, and optimizing load distribution. In this post, we discuss the challenges of integrating intermittent renewable energy sources and how modern DL models address them.",
                ImageUrl = "/uploads/smart-grid.png",
                Author = "Dr. Mansi Bhatnagar",
                Category = "Deep Learning",
                ReadTime = 7,
                CreatedDate = DateTime.Now.AddDays(-5)
            },
            new Article
            {
                Title = "The Intersection of AI, Research, and Academic Excellence",
                Slug = "intersection-of-ai-research-and-academic-excellence",
                Content = "Conducting research in AI requires a disciplined approach to experimentation, literature review, and collaboration. As AI technologies evolve at an unprecedented pace, staying relevant means focusing on fundamental mathematical concepts while applying them to meaningful societal problems. Here are some key tips for young researchers entering the AI domain.",
                ImageUrl = "/uploads/academic-research.png",
                Author = "Dr. Mansi Bhatnagar",
                Category = "Research",
                ReadTime = 4,
                CreatedDate = DateTime.Now.AddDays(-2)
            }
        });
    }

    // Seed GalleryImages
    if (!context.GalleryImages.Any())
    {
        context.GalleryImages.AddRange(new List<GalleryImage>
        {
            new GalleryImage
            {
                Title = "Keynote Speech at Tech Summit 2026",
                Description = "Presenting my research on Neural Networks in Smart Energy Infrastructures at the Global Tech Summit.",
                ImageUrl = "/uploads/keynote-speech.png",
                CreatedDate = DateTime.Now.AddDays(-30)
            },
            new GalleryImage
            {
                Title = "Smart Grid Research Lab",
                Description = "A view inside our main research facility where we test real-time grid telemetry anomaly detection algorithms.",
                ImageUrl = "/uploads/research-lab.png",
                CreatedDate = DateTime.Now.AddDays(-25)
            },
            new GalleryImage
            {
                Title = "Panel Discussion on AI Ethics",
                Description = "Discussing the role of ethical constraints in autonomous decision-making systems for power grids.",
                ImageUrl = "/uploads/panel-discussion.png",
                CreatedDate = DateTime.Now.AddDays(-15)
            }
        });
    }

    // Seed ContactMessages
    if (!context.ContactMessages.Any())
    {
        context.ContactMessages.AddRange(new List<ContactMessage>
        {
            new ContactMessage
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Subject = "Collaboration Inquiry",
                MessageText = "Hello Dr. Bhatnagar, I read your recent paper on Smart Grid Anomaly Detection and am highly interested in discussing potential research collaborations between our departments.",
                IsRead = false,
                CreatedDate = DateTime.Now.AddDays(-1)
            },
            new ContactMessage
            {
                FirstName = "Sarah",
                LastName = "Smith",
                Email = "sarah.smith@univ.edu",
                Subject = "Guest Lecture Invitation",
                MessageText = "Dear Dr. Mansi, we would be honored if you could deliver a guest lecture on Computer Vision applications to our postgraduate computer science students next month.",
                IsRead = true,
                CreatedDate = DateTime.Now.AddDays(-3)
            }
        });
    }

    // Seed PageSections
    if (!context.PageSections.Any())
    {
        var sections = new List<PageSection>();

        // ===== ABOUT PAGE =====
        sections.AddRange(new[]
        {
            new PageSection { PageName = "About", SectionKey = "about_hero_tagline", SectionType = "text", Title = "Hero Tagline", Content = "Welcome into my world", SortOrder = 1 },
            new PageSection { PageName = "About", SectionKey = "about_hero_name", SectionType = "text", Title = "Hero Name", Content = "Mansi Bhatnagar", SortOrder = 2 },
            new PageSection { PageName = "About", SectionKey = "about_hero_subtitle", SectionType = "text", Title = "Hero Subtitle", Content = "Ph.D. Scholar & AI Researcher", SortOrder = 3 },
            new PageSection { PageName = "About", SectionKey = "about_hero_image", SectionType = "image", Title = "Hero Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 4 },
            new PageSection { PageName = "About", SectionKey = "about_intro_heading", SectionType = "text", Title = "Introduction Heading", Content = "Bridging AI with Sustainable Infrastructure", SortOrder = 5 },
            new PageSection { PageName = "About", SectionKey = "about_intro_text", SectionType = "html", Title = "Introduction Text", Content = "<p>I hold a Master of Technology (MTech) in Cyber Security, which provided a strong foundation in protecting systems and managing digital infrastructures. Since then, my academic journey has evolved toward utilizing advanced AI techniques to solve real-world problems in energy distribution.</p><p>I am currently pursuing a Ph.D. at the Slovak University of Technology, focusing on Deep Learning methodologies and Computer Vision.</p><p>My current research focuses on Short-Term Electric Load Forecasting (STLF), exploring hybrid models and optimized feature selection techniques to improve the accuracy of energy management systems in smart microgrids.</p>", SortOrder = 6 },
            new PageSection { PageName = "About", SectionKey = "about_intro_image", SectionType = "image", Title = "Introduction Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 7 },
            new PageSection { PageName = "About", SectionKey = "about_stat_number", SectionType = "text", Title = "Stat Number", Content = "5+", SortOrder = 8 },
            new PageSection { PageName = "About", SectionKey = "about_stat_label", SectionType = "text", Title = "Stat Label", Content = "Years of AI Research", SortOrder = 9 },
            new PageSection { PageName = "About", SectionKey = "about_mission_heading", SectionType = "text", Title = "Mission Section Heading", Content = "My Core Mission", SortOrder = 10 },
            new PageSection { PageName = "About", SectionKey = "about_mission_subtitle", SectionType = "text", Title = "Mission Subtitle", Content = "Connecting research with real-world applications to tackle global resource challenges and support future innovators.", SortOrder = 11 },
            new PageSection { PageName = "About", SectionKey = "about_mission_1_title", SectionType = "text", Title = "Mission Card 1 Title", Content = "Research", SortOrder = 12 },
            new PageSection { PageName = "About", SectionKey = "about_mission_1_text", SectionType = "text", Title = "Mission Card 1 Text", Content = "Exploring innovative ML models and AI solutions for reliable short-term electric load forecasting and secure grid optimization.", SortOrder = 13 },
            new PageSection { PageName = "About", SectionKey = "about_mission_1_image", SectionType = "image", Title = "Mission Card 1 Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 14 },
            new PageSection { PageName = "About", SectionKey = "about_mission_2_title", SectionType = "text", Title = "Mission Card 2 Title", Content = "Mentorship", SortOrder = 15 },
            new PageSection { PageName = "About", SectionKey = "about_mission_2_text", SectionType = "text", Title = "Mission Card 2 Text", Content = "Fostering quantitative growth, programming confidence, and active opportunities for early-career researchers globally.", SortOrder = 16 },
            new PageSection { PageName = "About", SectionKey = "about_mission_2_image", SectionType = "image", Title = "Mission Card 2 Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 17 },
            new PageSection { PageName = "About", SectionKey = "about_mission_3_title", SectionType = "text", Title = "Mission Card 3 Title", Content = "Outreach", SortOrder = 18 },
            new PageSection { PageName = "About", SectionKey = "about_mission_3_text", SectionType = "text", Title = "Mission Card 3 Text", Content = "Engaging in active public communication to make data science, AI infrastructure, and cyber security accessible to everyone.", SortOrder = 19 },
            new PageSection { PageName = "About", SectionKey = "about_mission_3_image", SectionType = "image", Title = "Mission Card 3 Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 20 },
        });

        // ===== CV PAGE =====
        sections.AddRange(new[]
        {
            new PageSection { PageName = "CV", SectionKey = "cv_hero_title", SectionType = "text", Title = "CV Page Title", Content = "Curriculum Vitae", SortOrder = 1 },
            new PageSection { PageName = "CV", SectionKey = "cv_hero_subtitle", SectionType = "text", Title = "CV Subtitle", Content = "A journey of academic research, technological innovation, and continuous learning.", SortOrder = 2 },
            new PageSection { PageName = "CV", SectionKey = "cv_download_url", SectionType = "text", Title = "CV Download Link", Content = "#", SortOrder = 3 },
            // Education
            new PageSection { PageName = "CV", SectionKey = "cv_edu_1_date", SectionType = "text", Title = "Education 1 Date", Content = "2022 - Present", SortOrder = 10 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_1_title", SectionType = "text", Title = "Education 1 Title", Content = "Ph.D. in Deep Learning & Smart Grids", SortOrder = 11 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_1_institution", SectionType = "text", Title = "Education 1 Institution", Content = "Slovak University of Technology", SortOrder = 12 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_1_desc", SectionType = "text", Title = "Education 1 Description", Content = "Focusing on short-term electric load forecasting (STLF) using advanced meta-heuristic algorithms and Temporal Convolutional Networks.", SortOrder = 13 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_2_date", SectionType = "text", Title = "Education 2 Date", Content = "2018 - 2020", SortOrder = 14 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_2_title", SectionType = "text", Title = "Education 2 Title", Content = "MTech in Cyber Security", SortOrder = 15 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_2_institution", SectionType = "text", Title = "Education 2 Institution", Content = "Global Technology Institute", SortOrder = 16 },
            new PageSection { PageName = "CV", SectionKey = "cv_edu_2_desc", SectionType = "text", Title = "Education 2 Description", Content = "Specialized in deep network threat modeling, IoT vulnerabilities, and secure data distribution algorithms.", SortOrder = 17 },
            // Experience
            new PageSection { PageName = "CV", SectionKey = "cv_exp_1_date", SectionType = "text", Title = "Experience 1 Date", Content = "2023 - Present", SortOrder = 20 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_1_title", SectionType = "text", Title = "Experience 1 Title", Content = "Primary AI Researcher", SortOrder = 21 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_1_company", SectionType = "text", Title = "Experience 1 Company", Content = "International Smart Grid Consortium", SortOrder = 22 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_1_desc", SectionType = "text", Title = "Experience 1 Description", Content = "Leading analysis on load balancing models and publishing iterative findings in core engineering journals. Developing predictive data architectures.", SortOrder = 23 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_2_date", SectionType = "text", Title = "Experience 2 Date", Content = "2020 - 2022", SortOrder = 24 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_2_title", SectionType = "text", Title = "Experience 2 Title", Content = "Data Scientist", SortOrder = 25 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_2_company", SectionType = "text", Title = "Experience 2 Company", Content = "Tech Infrastructure Partners", SortOrder = 26 },
            new PageSection { PageName = "CV", SectionKey = "cv_exp_2_desc", SectionType = "text", Title = "Experience 2 Description", Content = "Designed anomaly detection prototypes for municipal energy setups to detect and mitigate usage spikes effectively.", SortOrder = 27 },
            // Skills
            new PageSection { PageName = "CV", SectionKey = "cv_skill_1_name", SectionType = "text", Title = "Skill 1 Name", Content = "Artificial Intelligence / ML", SortOrder = 30 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_1_percent", SectionType = "percentage", Title = "Skill 1 Percentage", Content = "95", SortOrder = 31 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_2_name", SectionType = "text", Title = "Skill 2 Name", Content = "Neural Networks & PyTorch", SortOrder = 32 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_2_percent", SectionType = "percentage", Title = "Skill 2 Percentage", Content = "92", SortOrder = 33 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_3_name", SectionType = "text", Title = "Skill 3 Name", Content = "Image Processing & CV", SortOrder = 34 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_3_percent", SectionType = "percentage", Title = "Skill 3 Percentage", Content = "88", SortOrder = 35 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_4_name", SectionType = "text", Title = "Skill 4 Name", Content = "Smart Grid Optimization", SortOrder = 36 },
            new PageSection { PageName = "CV", SectionKey = "cv_skill_4_percent", SectionType = "percentage", Title = "Skill 4 Percentage", Content = "90", SortOrder = 37 },
            // Publications
            new PageSection { PageName = "CV", SectionKey = "cv_publications_html", SectionType = "html", Title = "Publications List", Content = "<ul><li><strong>Using Crafted Features and Polar Bear Optimization Algorithm for STLF</strong><br/><em>2024</em></li><li><strong>Web Intrusion Classification System using Machine Learning</strong><br/><em>2022</em></li><li><strong>Comprehensive Electric load forecasting using ensemble methods</strong><br/><em>2022</em></li><li><strong>STLF Using Random Forest with Entropy-Based Feature Selection</strong><br/><em>2022</em></li><li><strong>AI in Agriculture: Efficient Plant Disease Detection</strong><br/><em>2022</em></li></ul>", SortOrder = 40 },
        });

        // ===== RESEARCH PAGE =====
        sections.AddRange(new[]
        {
            new PageSection { PageName = "Research", SectionKey = "research_hero_title", SectionType = "text", Title = "Research Page Title", Content = "Research", SortOrder = 1 },
            new PageSection { PageName = "Research", SectionKey = "research_1_heading", SectionType = "text", Title = "Research Section 1 Heading", Content = "Deep Learning & Smart Grid Optimization", SortOrder = 2 },
            new PageSection { PageName = "Research", SectionKey = "research_1_text", SectionType = "text", Title = "Research Section 1 Text", Content = "I believe intelligent systems can offer effective solutions to our energy crises. My research explores how deep learning architectures can accurately model complex networks, ultimately improving short-term electric load forecasting.", SortOrder = 3 },
            new PageSection { PageName = "Research", SectionKey = "research_1_image", SectionType = "image", Title = "Research Section 1 Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 4 },
            new PageSection { PageName = "Research", SectionKey = "research_2_heading", SectionType = "text", Title = "Research Section 2 Heading", Content = "Integrating Renewable Energy into Microgrids", SortOrder = 5 },
            new PageSection { PageName = "Research", SectionKey = "research_2_text", SectionType = "text", Title = "Research Section 2 Text", Content = "By leveraging advanced models like Temporal Convolutional Networks (TCN) paired with meta-heuristic algorithms (ACO, BSA), our team achieves highly accurate power forecasting, essential for integrating renewable energy.", SortOrder = 6 },
            new PageSection { PageName = "Research", SectionKey = "research_2_image", SectionType = "image", Title = "Research Section 2 Image", Content = "/staticimage/hero_section_img.jpg", SortOrder = 7 },
        });

        // ===== CONNECT LINKS =====
        sections.AddRange(new[]
        {
            new PageSection { PageName = "Connect", SectionKey = "connect_linkedin", SectionType = "text", Title = "LinkedIn Profile URL", Content = "https://linkedin.com/in/mansi-bhatnagar", SortOrder = 1 },
            new PageSection { PageName = "Connect", SectionKey = "connect_twitter", SectionType = "text", Title = "Twitter/X Profile URL", Content = "https://x.com/mansi_bhatnagar", SortOrder = 2 },
            new PageSection { PageName = "Connect", SectionKey = "connect_researchgate", SectionType = "text", Title = "ResearchGate Profile URL", Content = "https://researchgate.net/profile/Mansi-Bhatnagar", SortOrder = 3 },
            new PageSection { PageName = "Connect", SectionKey = "connect_github", SectionType = "text", Title = "GitHub Profile URL", Content = "https://github.com/mansi-bhatnagar", SortOrder = 4 },
        });

        context.PageSections.AddRange(sections);
    }

    if (!context.PageSections.Any(s => s.PageName == "Connect"))
    {
        context.PageSections.AddRange(new[]
        {
            new PageSection { PageName = "Connect", SectionKey = "connect_linkedin", SectionType = "text", Title = "LinkedIn Profile URL", Content = "https://linkedin.com/in/mansi-bhatnagar", SortOrder = 1 },
            new PageSection { PageName = "Connect", SectionKey = "connect_twitter", SectionType = "text", Title = "Twitter/X Profile URL", Content = "https://x.com/mansi_bhatnagar", SortOrder = 2 },
            new PageSection { PageName = "Connect", SectionKey = "connect_researchgate", SectionType = "text", Title = "ResearchGate Profile URL", Content = "https://researchgate.net/profile/Mansi-Bhatnagar", SortOrder = 3 },
            new PageSection { PageName = "Connect", SectionKey = "connect_github", SectionType = "text", Title = "GitHub Profile URL", Content = "https://github.com/mansi-bhatnagar", SortOrder = 4 },
        });
    }

    context.SaveChanges();
}

app.Run();

