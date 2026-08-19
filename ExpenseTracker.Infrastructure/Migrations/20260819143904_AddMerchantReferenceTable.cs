using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantReferenceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerchantId",
                table: "Expenses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "character varying(253)", maxLength: 253, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantAliases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchantId = table.Column<int>(type: "integer", nullable: false),
                    NormalizedAlias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantAliases_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "Id", "Name", "NormalizedName", "Website" },
                values: new object[,]
                {
                    { 1, "Tesco", "tesco", "tesco.com" },
                    { 2, "Sainsbury's", "sainsburys", "sainsburys.co.uk" },
                    { 3, "Asda", "asda", "asda.com" },
                    { 4, "Morrisons", "morrisons", "morrisons.com" },
                    { 5, "Aldi", "aldi", "aldi.co.uk" },
                    { 6, "Lidl", "lidl", "lidl.co.uk" },
                    { 7, "Co-op", "coop", "coop.co.uk" },
                    { 8, "Waitrose", "waitrose", "waitrose.com" },
                    { 9, "Marks & Spencer", "marksspencer", "marksandspencer.com" },
                    { 10, "Iceland", "iceland", "iceland.co.uk" },
                    { 11, "Ocado", "ocado", "ocado.com" },
                    { 12, "Booths", "booths", "booths.co.uk" },
                    { 13, "Farmfoods", "farmfoods", "farmfoods.co.uk" },
                    { 14, "Budgens", "budgens", "budgens.co.uk" },
                    { 15, "Nisa", "nisa", "nisalocally.co.uk" },
                    { 16, "Spar", "spar", "spar.co.uk" },
                    { 17, "Costcutter", "costcutter", "costcutter.co.uk" },
                    { 18, "Londis", "londis", "londis.co.uk" },
                    { 19, "Premier Stores", "premierstores", "premier-stores.co.uk" },
                    { 20, "Costco", "costco", "costco.co.uk" },
                    { 21, "Makro", "makro", "makro.co.uk" },
                    { 22, "Booker", "booker", "booker.co.uk" },
                    { 23, "Whole Foods Market", "wholefoodsmarket", "wholefoodsmarket.co.uk" },
                    { 24, "Holland & Barrett", "hollandbarrett", "hollandandbarrett.com" },
                    { 25, "Gousto", "gousto", "gousto.co.uk" },
                    { 26, "Shell", "shell", "shell.co.uk" },
                    { 27, "BP", "bp", "bp.com" },
                    { 28, "Esso", "esso", "esso.co.uk" },
                    { 29, "Texaco", "texaco", "texaco.co.uk" },
                    { 30, "Gulf", "gulf", "gulfoil.co.uk" },
                    { 31, "Jet", "jet", "jetlocal.co.uk" },
                    { 32, "Applegreen", "applegreen", "applegreenstores.com" },
                    { 33, "Murco", "murco", "murcopetroleum.co.uk" },
                    { 34, "Halfords", "halfords", "halfords.com" },
                    { 35, "Kwik Fit", "kwikfit", "kwik-fit.com" },
                    { 36, "ATS Euromaster", "atseuromaster", "atseuromaster.co.uk" },
                    { 37, "National Tyres", "nationaltyres", "national.co.uk" },
                    { 38, "Euro Car Parts", "eurocarparts", "eurocarparts.com" },
                    { 39, "RAC", "rac", "rac.co.uk" },
                    { 40, "AA", "aa", "theaa.com" },
                    { 41, "Costa Coffee", "costacoffee", "costa.co.uk" },
                    { 42, "Starbucks", "starbucks", "starbucks.co.uk" },
                    { 43, "Caffe Nero", "caffenero", "caffenero.com" },
                    { 44, "Pret A Manger", "pretamanger", "pret.co.uk" },
                    { 45, "Greggs", "greggs", "greggs.co.uk" },
                    { 46, "McDonald's", "mcdonalds", "mcdonalds.com" },
                    { 47, "Burger King", "burgerking", "burgerking.co.uk" },
                    { 48, "KFC", "kfc", "kfc.co.uk" },
                    { 49, "Subway", "subway", "subway.com" },
                    { 50, "Nando's", "nandos", "nandos.co.uk" },
                    { 51, "Wagamama", "wagamama", "wagamama.com" },
                    { 52, "Pizza Express", "pizzaexpress", "pizzaexpress.com" },
                    { 53, "Domino's", "dominos", "dominos.co.uk" },
                    { 54, "Pizza Hut", "pizzahut", "pizzahut.co.uk" },
                    { 55, "Papa John's", "papajohns", "papajohns.co.uk" },
                    { 56, "Five Guys", "fiveguys", "fiveguys.co.uk" },
                    { 57, "Wimpy", "wimpy", "wimpy.uk.com" },
                    { 58, "Leon", "leon", "leon.co.uk" },
                    { 59, "Itsu", "itsu", "itsu.com" },
                    { 60, "Wasabi", "wasabi", "wasabi.uk.com" },
                    { 61, "YO! Sushi", "yosushi", "yosushi.com" },
                    { 62, "Bella Italia", "bellaitalia", "bellaitalia.co.uk" },
                    { 63, "Zizzi", "zizzi", "zizzi.co.uk" },
                    { 64, "Ask Italian", "askitalian", "askitalian.co.uk" },
                    { 65, "Prezzo", "prezzo", "prezzorestaurants.co.uk" },
                    { 66, "TGI Fridays", "tgifridays", "tgifridays.co.uk" },
                    { 67, "Harvester", "harvester", "harvester.co.uk" },
                    { 68, "Toby Carvery", "tobycarvery", "tobycarvery.co.uk" },
                    { 69, "Beefeater", "beefeater", "beefeater.co.uk" },
                    { 70, "Miller & Carter", "millercarter", "millerandcarter.co.uk" },
                    { 71, "Wetherspoon", "wetherspoon", "jdwetherspoon.com" },
                    { 72, "Slug and Lettuce", "slugandlettuce", "slugandlettuce.co.uk" },
                    { 73, "Deliveroo", "deliveroo", "deliveroo.co.uk" },
                    { 74, "Just Eat", "justeat", "just-eat.co.uk" },
                    { 75, "Uber Eats", "ubereats", "ubereats.com" },
                    { 76, "Transport for London", "transportforlondon", "tfl.gov.uk" },
                    { 77, "Trainline", "trainline", "thetrainline.com" },
                    { 78, "National Rail", "nationalrail", "nationalrail.co.uk" },
                    { 79, "LNER", "lner", "lner.co.uk" },
                    { 80, "Avanti West Coast", "avantiwestcoast", "avantiwestcoast.co.uk" },
                    { 81, "GWR", "gwr", "gwr.com" },
                    { 82, "Southeastern", "southeastern", "southeasternrailway.co.uk" },
                    { 83, "Northern", "northern", "northernrailway.co.uk" },
                    { 84, "ScotRail", "scotrail", "scotrail.co.uk" },
                    { 85, "CrossCountry", "crosscountry", "crosscountrytrains.co.uk" },
                    { 86, "Thameslink", "thameslink", "thameslinkrailway.com" },
                    { 87, "Uber", "uber", "uber.com" },
                    { 88, "Bolt", "bolt", "bolt.eu" },
                    { 89, "Addison Lee", "addisonlee", "addisonlee.com" },
                    { 90, "National Express", "nationalexpress", "nationalexpress.com" },
                    { 91, "Megabus", "megabus", "megabus.com" },
                    { 92, "Stagecoach", "stagecoach", "stagecoachbus.com" },
                    { 93, "First Bus", "firstbus", "firstbus.co.uk" },
                    { 94, "Arriva", "arriva", "arrivabus.co.uk" },
                    { 95, "easyJet", "easyjet", "easyjet.com" },
                    { 96, "Ryanair", "ryanair", "ryanair.com" },
                    { 97, "British Airways", "britishairways", "britishairways.com" },
                    { 98, "Jet2", "jet2", "jet2.com" },
                    { 99, "TUI", "tui", "tui.co.uk" },
                    { 100, "Eurostar", "eurostar", "eurostar.com" },
                    { 101, "Amazon", "amazon", "amazon.co.uk" },
                    { 102, "eBay", "ebay", "ebay.co.uk" },
                    { 103, "Argos", "argos", "argos.co.uk" },
                    { 104, "John Lewis", "johnlewis", "johnlewis.com" },
                    { 105, "Next", "next", "next.co.uk" },
                    { 106, "Primark", "primark", "primark.com" },
                    { 107, "Debenhams", "debenhams", "debenhams.com" },
                    { 108, "House of Fraser", "houseoffraser", "houseoffraser.co.uk" },
                    { 109, "Selfridges", "selfridges", "selfridges.com" },
                    { 110, "Harrods", "harrods", "harrods.com" },
                    { 111, "TK Maxx", "tkmaxx", "tkmaxx.com" },
                    { 112, "Sports Direct", "sportsdirect", "sportsdirect.com" },
                    { 113, "JD Sports", "jdsports", "jdsports.co.uk" },
                    { 114, "Decathlon", "decathlon", "decathlon.co.uk" },
                    { 115, "Nike", "nike", "nike.com" },
                    { 116, "Adidas", "adidas", "adidas.co.uk" },
                    { 117, "Zara", "zara", "zara.com" },
                    { 118, "H&M", "hm", "hm.com" },
                    { 119, "Uniqlo", "uniqlo", "uniqlo.com" },
                    { 120, "River Island", "riverisland", "riverisland.com" },
                    { 121, "New Look", "newlook", "newlook.com" },
                    { 122, "ASOS", "asos", "asos.com" },
                    { 123, "Boohoo", "boohoo", "boohoo.com" },
                    { 124, "Shein", "shein", "shein.co.uk" },
                    { 125, "Currys", "currys", "currys.co.uk" },
                    { 126, "AO", "ao", "ao.com" },
                    { 127, "Apple", "apple", "apple.com" },
                    { 128, "Samsung", "samsung", "samsung.com" },
                    { 129, "Game", "game", "game.co.uk" },
                    { 130, "Smyths Toys", "smythstoys", "smythstoys.com" },
                    { 131, "B&Q", "bq", "diy.com" },
                    { 132, "Screwfix", "screwfix", "screwfix.com" },
                    { 133, "Wickes", "wickes", "wickes.co.uk" },
                    { 134, "Homebase", "homebase", "homebase.co.uk" },
                    { 135, "Toolstation", "toolstation", "toolstation.com" },
                    { 136, "Travis Perkins", "travisperkins", "travisperkins.co.uk" },
                    { 137, "Jewson", "jewson", "jewson.co.uk" },
                    { 138, "IKEA", "ikea", "ikea.com" },
                    { 139, "Dunelm", "dunelm", "dunelm.com" },
                    { 140, "The Range", "therange", "therange.co.uk" },
                    { 141, "Wilko", "wilko", "wilko.com" },
                    { 142, "Home Bargains", "homebargains", "homebargains.co.uk" },
                    { 143, "B&M", "bm", "bmstores.co.uk" },
                    { 144, "Poundland", "poundland", "poundland.co.uk" },
                    { 145, "Poundstretcher", "poundstretcher", "poundstretcher.co.uk" },
                    { 146, "Robert Dyas", "robertdyas", "robertdyas.co.uk" },
                    { 147, "Wayfair", "wayfair", "wayfair.co.uk" },
                    { 148, "Made", "made", "made.com" },
                    { 149, "DFS", "dfs", "dfs.co.uk" },
                    { 150, "Dobbies", "dobbies", "dobbies.com" },
                    { 151, "Boots", "boots", "boots.com" },
                    { 152, "Superdrug", "superdrug", "superdrug.com" },
                    { 153, "Lloyds Pharmacy", "lloydspharmacy", "lloydspharmacy.com" },
                    { 154, "Well Pharmacy", "wellpharmacy", "well.co.uk" },
                    { 155, "The Body Shop", "thebodyshop", "thebodyshop.com" },
                    { 156, "Lush", "lush", "lush.com" },
                    { 157, "Sephora", "sephora", "sephora.co.uk" },
                    { 158, "Space NK", "spacenk", "spacenk.com" },
                    { 159, "Specsavers", "specsavers", "specsavers.co.uk" },
                    { 160, "Vision Express", "visionexpress", "visionexpress.com" },
                    { 161, "Bupa", "bupa", "bupa.co.uk" },
                    { 162, "Nuffield Health", "nuffieldhealth", "nuffieldhealth.com" },
                    { 163, "PureGym", "puregym", "puregym.com" },
                    { 164, "The Gym Group", "thegymgroup", "thegymgroup.com" },
                    { 165, "David Lloyd", "davidlloyd", "davidlloyd.co.uk" },
                    { 166, "BT", "bt", "bt.com" },
                    { 167, "EE", "ee", "ee.co.uk" },
                    { 168, "Vodafone", "vodafone", "vodafone.co.uk" },
                    { 169, "O2", "o2", "o2.co.uk" },
                    { 170, "Three", "three", "three.co.uk" },
                    { 171, "Sky", "sky", "sky.com" },
                    { 172, "Virgin Media", "virginmedia", "virginmedia.com" },
                    { 173, "TalkTalk", "talktalk", "talktalk.co.uk" },
                    { 174, "Plusnet", "plusnet", "plus.net" },
                    { 175, "Giffgaff", "giffgaff", "giffgaff.com" },
                    { 176, "Tesco Mobile", "tescomobile", "tescomobile.com" },
                    { 177, "British Gas", "britishgas", "britishgas.co.uk" },
                    { 178, "EDF Energy", "edfenergy", "edfenergy.com" },
                    { 179, "E.ON", "eon", "eonnext.com" },
                    { 180, "Octopus Energy", "octopusenergy", "octopus.energy" },
                    { 181, "OVO Energy", "ovoenergy", "ovoenergy.com" },
                    { 182, "Scottish Power", "scottishpower", "scottishpower.co.uk" },
                    { 183, "SSE", "sse", "sse.co.uk" },
                    { 184, "Thames Water", "thameswater", "thameswater.co.uk" },
                    { 185, "Severn Trent", "severntrent", "stwater.co.uk" },
                    { 186, "Netflix", "netflix", "netflix.com" },
                    { 187, "Spotify", "spotify", "spotify.com" },
                    { 188, "Disney+", "disney", "disneyplus.com" },
                    { 189, "Amazon Prime Video", "amazonprimevideo", "primevideo.com" },
                    { 190, "NOW", "now", "nowtv.com" },
                    { 191, "Apple Music", "applemusic", "music.apple.com" },
                    { 192, "YouTube", "youtube", "youtube.com" },
                    { 193, "Audible", "audible", "audible.co.uk" },
                    { 194, "Adobe", "adobe", "adobe.com" },
                    { 195, "Microsoft", "microsoft", "microsoft.com" },
                    { 196, "Google", "google", "google.com" },
                    { 197, "Dropbox", "dropbox", "dropbox.com" },
                    { 198, "PayPal", "paypal", "paypal.com" },
                    { 199, "Klarna", "klarna", "klarna.com" },
                    { 200, "TV Licensing", "tvlicensing", "tvlicensing.co.uk" }
                });

            migrationBuilder.InsertData(
                table: "MerchantAliases",
                columns: new[] { "Id", "MerchantId", "NormalizedAlias" },
                values: new object[,]
                {
                    { 1, 1, "tescostores" },
                    { 2, 1, "tescostoreslimited" },
                    { 3, 1, "tescoexpress" },
                    { 4, 1, "tescoextra" },
                    { 5, 1, "tescometro" },
                    { 6, 1, "tescosuperstore" },
                    { 7, 1, "tescopetrolstation" },
                    { 8, 2, "sainsbury" },
                    { 9, 2, "sainsburyslocal" },
                    { 10, 2, "jsainsburyplc" },
                    { 11, 2, "sainsburyssuperstore" },
                    { 12, 3, "asdastores" },
                    { 13, 3, "asdasuperstore" },
                    { 14, 3, "asdaliving" },
                    { 15, 4, "morrisonsdaily" },
                    { 16, 4, "wmmorrisons" },
                    { 17, 5, "aldistores" },
                    { 18, 6, "lidlgb" },
                    { 19, 6, "lidluk" },
                    { 20, 7, "coopfood" },
                    { 21, 7, "thecooperativefood" },
                    { 22, 7, "cooperativefood" },
                    { 23, 8, "waitrosepartners" },
                    { 24, 9, "ms" },
                    { 25, 9, "marksandspencer" },
                    { 26, 9, "mssimplyfood" },
                    { 27, 9, "msfoodhall" },
                    { 28, 10, "icelandfoods" },
                    { 29, 24, "hollandandbarrett" },
                    { 30, 26, "shelluk" },
                    { 31, 26, "shellukoil" },
                    { 32, 26, "shellservicestation" },
                    { 33, 27, "bpconnect" },
                    { 34, 27, "bpfuel" },
                    { 35, 27, "bppetrol" },
                    { 36, 28, "essoexpress" },
                    { 37, 28, "essopetrolstation" },
                    { 38, 34, "halfordsautocentre" },
                    { 39, 35, "kwikfitplus" },
                    { 40, 41, "costa" },
                    { 41, 41, "costaexpress" },
                    { 42, 41, "costalimited" },
                    { 43, 42, "starbuckscoffee" },
                    { 44, 43, "nerocaffe" },
                    { 45, 44, "pret" },
                    { 46, 45, "greggsplc" },
                    { 47, 45, "greggsthebakers" },
                    { 48, 46, "mcds" },
                    { 49, 46, "macdonalds" },
                    { 50, 47, "burgerkinguk" },
                    { 51, 48, "kentuckyfriedchicken" },
                    { 52, 50, "nandoschickenland" },
                    { 53, 53, "dominospizza" },
                    { 54, 54, "pizzahutdelivery" },
                    { 55, 55, "papajohnspizza" },
                    { 56, 56, "fiveguysburgersandfries" },
                    { 57, 71, "jdwetherspoon" },
                    { 58, 71, "wetherspoons" },
                    { 59, 74, "justeattakeaway" },
                    { 60, 75, "ubereatsuk" },
                    { 61, 76, "tfl" },
                    { 62, 76, "tfltravelcharge" },
                    { 63, 76, "tflrail" },
                    { 64, 76, "oystercard" },
                    { 65, 77, "thetrainline" },
                    { 66, 77, "trainlinecom" },
                    { 67, 81, "greatwesternrailway" },
                    { 68, 87, "uberbv" },
                    { 69, 87, "ubertrip" },
                    { 70, 87, "uberuk" },
                    { 71, 90, "nationalexpressltd" },
                    { 72, 95, "easyjetairline" },
                    { 73, 101, "amazonuk" },
                    { 74, 101, "amazoncouk" },
                    { 75, 101, "amazonmarketplace" },
                    { 76, 101, "amznmktplace" },
                    { 77, 101, "amazondigital" },
                    { 78, 102, "ebayuk" },
                    { 79, 103, "argosltd" },
                    { 80, 103, "argosretail" },
                    { 81, 104, "johnlewispartners" },
                    { 82, 104, "johnlewispartnership" },
                    { 83, 111, "tkmaxxuk" },
                    { 84, 113, "jdsport" },
                    { 85, 125, "curryspcworld" },
                    { 86, 125, "pcworld" },
                    { 87, 125, "currysdigital" },
                    { 88, 127, "applestore" },
                    { 89, 127, "appleuk" },
                    { 90, 131, "bandq" },
                    { 91, 131, "bqltd" },
                    { 92, 132, "screwfixdirect" },
                    { 93, 133, "wickesbuildingsupplies" },
                    { 94, 138, "ikealtd" },
                    { 95, 138, "ikeauk" },
                    { 96, 142, "tjmorris" },
                    { 97, 143, "bmbargains" },
                    { 98, 143, "bmstores" },
                    { 99, 153, "lloydspharmacyltd" },
                    { 100, 163, "puregymlimited" },
                    { 101, 166, "btgroup" },
                    { 102, 166, "btbroadband" },
                    { 103, 167, "eelimited" },
                    { 104, 167, "eemobile" },
                    { 105, 168, "vodafoneuk" },
                    { 106, 168, "vodafoneltd" },
                    { 107, 169, "o2uk" },
                    { 108, 169, "telefonicao2" },
                    { 109, 170, "threeuk" },
                    { 110, 170, "hutchison3g" },
                    { 111, 171, "skyuk" },
                    { 112, 171, "skydigital" },
                    { 113, 171, "skysubscription" },
                    { 114, 172, "virginmediauk" },
                    { 115, 177, "britishgastrading" },
                    { 116, 177, "britishgasenergy" },
                    { 117, 178, "edf" },
                    { 118, 178, "edfenergyuk" },
                    { 119, 179, "eonnext" },
                    { 120, 180, "octopusenergyltd" },
                    { 121, 181, "ovoenergyltd" },
                    { 122, 182, "scottishpowerenergy" },
                    { 123, 184, "thameswaterutilities" },
                    { 124, 185, "severntrentwater" },
                    { 125, 186, "netflixcom" },
                    { 126, 186, "netflixuk" },
                    { 127, 187, "spotifyuk" },
                    { 128, 187, "spotifyab" },
                    { 129, 188, "disneyplus" },
                    { 130, 188, "disneyuk" },
                    { 131, 189, "primevideo" },
                    { 132, 189, "amazonprime" },
                    { 133, 190, "nowtv" },
                    { 134, 191, "itunes" },
                    { 135, 192, "youtubepremium" },
                    { 136, 192, "googleyoutube" },
                    { 137, 193, "audibleuk" },
                    { 138, 194, "adobesystems" },
                    { 139, 194, "adobecreativecloud" },
                    { 140, 195, "microsoftcorporation" },
                    { 141, 195, "msft" },
                    { 142, 196, "googlepayment" },
                    { 143, 196, "googleireland" },
                    { 144, 196, "googleplay" },
                    { 145, 198, "paypaluk" },
                    { 146, 198, "paypalinst" },
                    { 147, 200, "tvlicence" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_MerchantId",
                table: "Expenses",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantAliases_MerchantId",
                table: "MerchantAliases",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantAliases_NormalizedAlias",
                table: "MerchantAliases",
                column: "NormalizedAlias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_NormalizedName",
                table: "Merchants",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Merchants_MerchantId",
                table: "Expenses",
                column: "MerchantId",
                principalTable: "Merchants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // HasData inserts explicit ids without advancing the identity sequence, so without this
            // the first merchant a user creates collides with a seeded id.
            migrationBuilder.Sql(ResetMerchantSequences);

            // Backfill the FK from the free-text column before it is dropped. The normalization
            // here must match Merchant.Normalize in the domain layer.
            migrationBuilder.Sql(CreateMerchantsForUnknownNames);
            migrationBuilder.Sql(LinkExpensesByName);
            migrationBuilder.Sql(LinkExpensesByAlias);
            migrationBuilder.Sql(ResetMerchantSequences);

            migrationBuilder.DropColumn(
                name: "Merchant",
                table: "Expenses");
        }

        private const string ResetMerchantSequences = """
            SELECT setval(pg_get_serial_sequence('"Merchants"', 'Id'),
                          GREATEST((SELECT COALESCE(MAX("Id"), 1) FROM "Merchants"), 1));
            SELECT setval(pg_get_serial_sequence('"MerchantAliases"', 'Id'),
                          GREATEST((SELECT COALESCE(MAX("Id"), 1) FROM "MerchantAliases"), 1));
            """;

        /// <summary>
        /// Any merchant string that matches neither a seeded name nor an alias becomes its own
        /// row, with the website left unknown. One representative spelling is kept for display.
        /// </summary>
        private const string CreateMerchantsForUnknownNames = """
            INSERT INTO "Merchants" ("Name", "NormalizedName", "Website")
            SELECT MIN(trim(e."Merchant")), regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g'), NULL
            FROM "Expenses" e
            WHERE e."Merchant" IS NOT NULL
              AND regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g') <> ''
              AND NOT EXISTS (SELECT 1 FROM "Merchants" m WHERE m."NormalizedName" = regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g'))
              AND NOT EXISTS (SELECT 1 FROM "MerchantAliases" a WHERE a."NormalizedAlias" = regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g'))
            GROUP BY regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g');
            """;

        private const string LinkExpensesByName = """
            UPDATE "Expenses" e
            SET "MerchantId" = m."Id"
            FROM "Merchants" m
            WHERE e."Merchant" IS NOT NULL
              AND m."NormalizedName" = regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g');
            """;

        private const string LinkExpensesByAlias = """
            UPDATE "Expenses" e
            SET "MerchantId" = a."MerchantId"
            FROM "MerchantAliases" a
            WHERE e."MerchantId" IS NULL
              AND e."Merchant" IS NOT NULL
              AND a."NormalizedAlias" = regexp_replace(lower(trim(e."Merchant")), '[^a-z0-9]', '', 'g');
            """;

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Merchants_MerchantId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "MerchantAliases");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_MerchantId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "Expenses");

            migrationBuilder.AddColumn<string>(
                name: "Merchant",
                table: "Expenses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
