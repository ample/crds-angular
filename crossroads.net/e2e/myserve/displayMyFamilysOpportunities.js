var env = require("../environment");
var moment = require('moment');

describe('Crossroads App', function() {

  beforeEach(function(){
    browser.get(env.baseUrl + '/#/'); 
    var loginButton = element.all(by.css(".navbar--login")).get(0).all(by.buttonText('Login'));
    expect(loginButton.count()).toBe(2); 
    expect(loginButton.get(0).isDisplayed()).toBeTruthy();
    loginButton.get(0).click();
   
    var emailInput = element.all(by.css(".navbar--login")).get(0).element(by.id("login-dropdown-email"));
    var passwordInput = element.all(by.css(".navbar--login")).get(0).element(by.id("login-dropdown-password"));
    var submitBtn = element.all(by.css(".navbar--login")).get(0).all(by.buttonText("Login")).get(1);
   
    emailInput.sendKeys("lakshmi.maramraju@gmail.com");
    passwordInput.sendKeys("123456");
    submitBtn.click(); 
  });

  afterEach(function(){ 
    var loginButton = element.all(by.css(".navbar--login")).get(0).all(by.buttonText('Login'));
    var logoutButton = element.all(by.css(".navbar--login")).get(0).all(by.linkText('Sign Out'));
    logoutButton.click();
    expect(loginButton.get(0).isDisplayed()).toBeTruthy();
  });


  it('should go the serve signup page', function() {  
    expect(element(by.id("current-user")).getText()).toBe("Laks");  
    browser.get(env.baseUrl + "/#/serve-signup");
    expect(element.all(by.css(".page-header")).get(0).getText()).toBe("Sign Up To Serve");
    var today = moment();
    element.all(by.css(".serve-day")).then(function(days){
      expect(days.length).toBeGreaterThan(0);
      var onPage = days[0].element(by.css('h4')).getText();
      var onPageDate = moment(onPage, 'EEEE, MMMM dd, yyyy');
      expect(onPageDate.dayOfYear()).toBeGreaterThan(today.dayOfYear() - 1);
    });
  }); 

  

});
