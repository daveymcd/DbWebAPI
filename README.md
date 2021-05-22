# DbWebAPI 
ASP.Net Core Web Service Demo

     DbWebApi v1.2 - Database Web Service for archival of documents.
 
     V1.0 2021-04-01 D.McDonald 
     Creation of Web API controller for use by Xamarin Mobile App. 
     
     V1.1 2021-04-14 D.McDonald
     Added simple View controller for browser access.
     
     V1.2 2021-04-17 D.McDonald
     Added Razor Pages with sort, search and modal CRUD facility.
 
Overview:

     The in-memory Database holds mock-up's of various food industry 
     regulatory documents. The governments 'Food Standards Agency' 
     require these documents to be archived and held by catering 
     companies as a record of their compliance with UK food hygiene 
     regulation...
     
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
                               To record the hott-holding 
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

     A Xamarin Mobile Application manages the regulatory reqiurements 
     of the business and uses this REST Api to store and retrieve the 
     users documents.
     
     * SCxItem.cs is the document archive Class, holding the Food Hygiene 
       Document data. Each Documents is TimeStamped and Typed. 
     
     * SCxItemController.cs is The API's endpoints controller and services 
       the CRUD requests.
       
     * SCxViewController.cs is The MVC View controller.
    
     "http://.../Home" is the API's landing page and offers access to 
     Swagger, MVC Views and Razor Pages. The latter 2 options were added 
     to extend the web services offered by the API. 
     
For the Open API Web Service please see...

     DbWebAPI.Controllers.SCxItemsController.cs
     DbWebAPI.Models.SCxItems.cs

For the MVC View Web Service (Views project folder) please see...

      DbWebAPI.Controllers.SCxViewController.cs
      DbWebAPI.Models.SCxItems.cs
      DbWebAPI.Views.SCxView.Index.cshtml

For the Razor Page Web Service (Pages project folder) please see...
    
        DbWebAPI.Models.SCxItems.cs
        DbWebAPI.Pages.Index.cshtml.cs
        DbWebAPI.Pages.Shared._PopupEdit.cshtml
        
        
        
