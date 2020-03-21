# Hotels-Admin-Panel

The site would start with the process of creating the purchase system for the user manager, which would choose what item he wants in stock and how much from the specific item he wants.
There would be a search bar for a faster finding in the items, categories which would help you find the specific items in a specific category and a very easy to use interface of functions.
There would be only a few steps to be done by the manager and the sites focuses on simple and fast to use interface. There would be an easy steps which are – create, add items and confirm.
With pressing the create button you are starting the process, then you have the add items button. You can easily find items by typing the item that you are looking for. By clicking the “add” button in the table of items you are adding it to the list for later confirmation.
When finding the items that a manager would like to order he clicks on confirm order, which gives him a window asking him to confirm the requisition, by clicking yes, the system is sending it to the logistics manager.
When the logistics manager logs in the system, he would see that he has one order for approvement. He would click on the order and see – the name of the requester, which hotel the order is coming from, what item, SKU, quantity, price in DKK, category and supplier.
He has the option to decline a specific item and approve only several items in the order. By seeing the items and making his choice he could approve or decline the order. By the logistics manager clicking confirm an automatic e-mail has been send to the requester that the order has been processed an approved.
When the day for delivery comes the logistics manager is logging in to the system and  by clicking confirm and send email to suppliers the logistics manager has send the items that are needed. And the supplier proceeds with delivery.

### Installing

You can deploy this site in azure with a few steps.

* Create new resource group in azure (web + sql);
* Run this project in visual studio
* Go to appsettings.json and change DefaultConnection string with yours. Also change default admin settings with yours.
* Now you are ready to start this project for first time, so go to azure and download publish profile.
* Go to visual studio and right click on AdminPanelApp and click publish
* Clcik on Import Profile and upload that profile from azure.
* Click on publish and enjoy.

### Features

* Asp.Net Core Mvc
* EF / Entity Framework core
* Code First
* C#
* Beautiful Bootstrap
* Html5 + css3
* JQuery + Ajax

### What features does this panel currently have?

* Managers
  * Ability to view history of their latest requisitions information (Approved or Rejected);
  * Ability to create new requisition. Adding products to requsition and confirm or delete requisition.
* Admin
  * Ability to approve or reject requisition. Automatic email will be send to manager with approved requisition details.
  * Ability to add/remove products
  * Ability to add/remove suppliers
  * Ability to register new managers
  * Ability to change supplier of a product
  * Ability to change supplier emails timer
  * Ability to approve supplier emails
  * Ability to view and download details for previous month requisitons (Requisitions history)

### Development Tools & Environment

* Visual Studio 2019 (Enterprise Edition). [(https://visualstudio.microsoft.com/)](https://visualstudio.microsoft.com/)

## Functional Features in Development

* Extending IdentityUser
