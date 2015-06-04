var env = require("../environment");

describe('Giving Flow', function() {

  beforeEach(function() {
    browser.get(env.baseUrl + '/#/give');
  })

  afterEach(function() {
    var logoutButton = element.all(by.css(".navbar--login")).get(0).all(by.linkText('Sign Out'));
    logoutButton.click();
  });

  it('should follow full credit card flow, logging in as user with existing giver', function () {
    expect(browser.getCurrentUrl()).toMatch(/\/amount/);
    element(by.model('amount')).sendKeys("12345");
    element(by.binding('amount')).click();
    expect(browser.getCurrentUrl()).toMatch(/\/login/);
    var loginButton = element.all(by.css('.btn')).get(5);
    expect(loginButton.getText()).toBe("Login");
    loginButton.click();
    element(by.id('login-page-email')).sendKeys("tim@kriz.net");
    element(by.id('login-page-password')).sendKeys("password");
    var button = element.all(by.id('submit_nav')).get(2);
    button.click();
    expect(browser.getCurrentUrl()).toMatch(/\/confirm/);
    var giveButton = element(by.css("[ng-click=\"give.confirmDonation()\"]"));
    expect(giveButton.getText()).toBe("GIVE $12,345.00");
    giveButton.click();
    expect(browser.getCurrentUrl()).toMatch(/\/thank-you/);
  });

  it('should follow full  flow, giving as guest', function () {
    expect(browser.getCurrentUrl()).toMatch(/\/amount/);
    element(by.model('amount')).sendKeys("1999");
    element(by.binding('amount')).click();
    expect(browser.getCurrentUrl()).toMatch(/\/login/);
    var giveAsGuestButton = element.all(by.css('.btn')).get(6);
    expect(giveAsGuestButton.getText()).toBe("Give as Guest");
    giveAsGuestButton.click();
    var creditCardButton = element.all(by.model('give.dto.view')).get(1);
    expect(creditCardButton.getText()).toBe("Credit Card");
    creditCardButton.click();
    element(by.id('give-email')).sendKeys("tim@kriz.net");
    element(by.model('creditCard.nameOnCard')).sendKeys("Mr Cross Roads");
    element(by.model('creditCard.ccNumber')).sendKeys("4242424242424242");
    element(by.model('creditCard.expDate')).sendKeys("0118");
    element(by.model('creditCard.cvc')).sendKeys("654");
    element(by.model('creditCard.billingZipCode')).sendKeys("45202-5236");

    var giveButton = element.all(by.css("[ng-click=\"give.submitBankInfo()\"]")).get(0);
    expect(giveButton.getText()).toBe("GIVE $1,999.00");

    giveButton.click().then(function() {
      expect(browser.getCurrentUrl()).toMatch(/\/thank-you/);
      var email = element.all(by.binding('give.email')).first();
      expect(email.getText()).toBe("tim@kriz.net");
    });
  });

  it('should register as new user and not lose the amt or fund', function () {
    expect(browser.getCurrentUrl()).toMatch(/\/amount/);
    element(by.model('amount')).sendKeys("867539");
    expect(element(by.binding("amount")).getText()).toContain("GIVE $867,539.00");
    element(by.binding('amount')).click();
    expect(browser.getCurrentUrl()).toMatch(/\/login/);
    var regButton = element(by.linkText('Create an account'));
    regButton.click();
    expect(browser.getCurrentUrl()).toMatch(/\/register/);
    element.all(by.id('registration-firstname')).get(1).sendKeys("Jack");
    element.all(by.id('registration-lastname')).get(1).sendKeys("Protractor");
    var ranNum = Math.floor((Math.random() * 1000) + 1);
    element.all(by.id('registration-email')).get(2).sendKeys("updates+" +ranNum+ "@crossroads.net");
    element.all(by.id('registration-password')).get(2).sendKeys("protractor");
    var regButton = element.all(by.css('.btn')).get(5);
    regButton.click();
    expect(browser.getCurrentUrl()).toMatch(/\/account/);
    element(by.cssContainingText('.ng-binding', 'Ministry'));
    element(by.cssContainingText('.ng-binding', '$867,539.00'));
  });

  it('should follow full credit card flow, logging in as user with existing giver and changing account information', function () {
    expect(browser.getCurrentUrl()).toMatch(/\/amount/);
    element(by.model('amount')).sendKeys("12345");
    element(by.binding('amount')).click();
    expect(browser.getCurrentUrl()).toMatch(/\/login/);
    var loginButton = element.all(by.css('.btn')).get(5);
    expect(loginButton.getText()).toBe("Login");
    loginButton.click();
    element(by.id('login-page-email')).sendKeys("tim@kriz.net");
    element(by.id('login-page-password')).sendKeys("password");
    var button = element.all(by.id('submit_nav')).get(2);
    button.click();
    expect(browser.getCurrentUrl()).toMatch(/\/confirm/);
    var giveButton = element(by.css("[ng-click=\"give.goToChange(give.amount, give.donor, give.email, give.program, 'cc')\"]"));
    giveButton.click();
    expect(browser.getCurrentUrl()).toMatch(/\/change/);
    var creditCardButton = element.all(by.model('give.dto.view')).get(1);
    expect(creditCardButton.getText()).toBe("Credit Card");
    creditCardButton.click();
    element(by.model('amount')).clear();
    element(by.model('amount')).sendKeys("54321");
    element(by.model('creditCard.nameOnCard')).sendKeys("Mr Change Cards");
    element(by.model('creditCard.ccNumber')).sendKeys("5555555555554444");
    element(by.model('creditCard.expDate')).sendKeys("0818");
    element(by.model('creditCard.cvc')).sendKeys("999");
    element(by.model('creditCard.billingZipCode')).sendKeys("45202-0818");
    var chgButton = element.all(by.css("[ng-click=\"give.submitChangedBankInfo()\"]")).get(0);
    expect(chgButton.getText()).toBe("GIVE $54,321.00");
    chgButton.click().then(function() {
      browser.waitForAngular();
      expect(browser.getCurrentUrl()).toMatch(/\/thank-you/);
      var email = element.all(by.binding('give.email')).first();
      expect(email).toBeDefined();
      expect(email.getText()).toBe("tim@kriz.net");

      var amount = element.all(by.binding('give.amount')).first();
      expect(amount).toBeDefined();
      expect(amount.getText()).toBe("$54,321.00");

      var program = element.all(by.binding("give.program['Name']")).first();
      expect(program).toBeDefined();
      expect(program.getText()).toBe("Crossroads");
    });
  });
})
