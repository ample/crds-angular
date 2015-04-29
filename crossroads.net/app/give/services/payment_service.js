(function () {

  module.exports = PaymentService;

  function PaymentService($log, $http, $q, stripe) {
    var payment_service = {
      donor : {},
      createDonorWithCard : createDonorWithCard
    };
    
    stripe.setPublishableKey(__STRIPE_PUBKEY__);
    
    function createDonorWithCard(card) {
      var def = $q.defer();    
      stripe.card.createToken(card)
        .then(function (token) {
          var donor_request = {
            tokenId: token.id
          }
          $http.post(__API_ENDPOINT__ + 'api/donor', donor_request)
            .success(function(data) {
              payment_service.donor = data;
              def.resolve(data);
            })
            .error(function(data) {
              def.reject(data.message);
            });
        });
        
      return def.promise;
    }
    
    return payment_service;
  }

})();