# DbWebAPI 
ASP.Net Core Web Service Demo

     DbWebApi v1.2 - Database Web Service for archival of documents.
 
     V1.0 2021-04-01 D.McDonald 
     Creation of REST API controller for use by Xamarin Mobile App. 
     
     V1.1 2021-04-14 D.McDonald
     Added simple MVC View controller for browser access.
     
     V1.2 2021-04-17 D.McDonald
     Added Razor Pages with sort, search and modal CRUD facility.
 
Overview:

     This demonstration of an OpenAPI Web Service uses an in-memory  
     Database to hold mock-up's of various food industry regulatory 
     documents. 
     
     The governments 'Food Standards Agency' require these documents 
     to be archived and held by catering companies as a record of their 
     compliance with UK food hygiene regulation.
     
Documents:

     SC1: Deliveries In      – Food Delivery Record.                
                               To record the monitoring of incoming 
                               deliveries.
                               (high risk, ready-to-eat food only).
         
     SC2: Chiller Checks     – Fridge/Cold room/Display Chiller 
                               Temperature records. 
                               To record the monitoring of the 
                               chill units, refrigerator's, cold
                               units (and the function of freezer's).
               
     SC3: Cooking Log        – Cooking/Cooling/Reheating Records. 
                               To record the monitoring of cooking, 
                               cooling and reheating temperatures.
                               
     SC4: Hot-Holding        – Hot Hold/Display Records. 
                               To record the hot holding 
                               temperatures of food.
                                   
     SC5: Hygiene Inspection – Hygiene Inspection Checklist. 
                               To record managers/supervisors checks 
                               of premises.
                               
     SC6: Hygiene Training   – Hygiene Training Records. 
                               To record training of staff.
                               
     SC7: Fitness To Work    – Fitness to Work Assessment Form. 
                               To record assessment of staff 
                               fitness to work.
                               
     SC8: All-In-One Form    – All-in-one Record. 
                               An alternative to SC1 - SC4 (not used).
                               
     SC9: Deliveries Out     – Customer Delivery Record. 
                               To record monitoring of food deliveries 
                               out to customers.
                               
     COP: Opening Checks     - Daily opening checks by supervisor.
     
     CCL: Closing Checks     - Daily closing checks by supervisor.
     
Notes:

     A Xamarin mobile application manages the business requirements 
     of the regulation and uses the REST API service to access the 
     document archive.
     
     * SCxItem.cs is the document archive Class, holding the food hygiene 
       document data. Each document is time stamped and typed. 
     
     * SCxItemController.cs is The REST API's endpoints controller and 
       services the CRUD requests.
       
     * SCxViewController.cs is The MVC View controller.
    
     "http://.../Home" is the API's landing page and offers access to 
     Swagger, MVC Views and Razor Pages. The latter 2 options were added 
     to extend the web services offered by the API. 
     
Model:

![image](https://user-images.githubusercontent.com/39599997/119809008-9497f000-bedc-11eb-9d57-df750d56de90.png)

For the REST API Web Service code please see...

     DbWebAPI.Controllers.SCxItemsController.cs
     DbWebAPI.Models.SCxItems.cs

For the MVC View Web Service code (Views project folder) please see...

      DbWebAPI.Controllers.SCxViewController.cs
      DbWebAPI.Models.SCxItems.cs
      DbWebAPI.Views.SCxView.Index.cshtml

For the Razor Page Web Service code (Pages project folder) please see...
    
        DbWebAPI.Models.SCxItems.cs
        DbWebAPI.Pages.Index.cshtml.cs
        DbWebAPI.Pages.Shared._PopupEdit.cshtml
        
HomePage:

![Home](https://user-images.githubusercontent.com/39599997/119225080-a30b9380-baf9-11eb-85c1-c57f606d4dd3.JPG)

