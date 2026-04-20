(function() {
    var whatsappNumber = document.currentScript.getAttribute('data-number') || '5511999999999';
    var message = encodeURIComponent('Olá! Gostaria de fazer um pedido ou tirar uma dúvida.');
    var whatsappUrl = 'https://wa.me/' + whatsappNumber + '?text=' + message;
    
    var btn = document.createElement('a');
    btn.href = whatsappUrl;
    btn.target = '_blank';
    btn.className = 'whatsapp-float';
    btn.style.cssText = 'position:fixed; bottom:20px; right:20px; background-color:#25d366; color:white; border-radius:50px; padding:12px 18px; display:flex; align-items:center; gap:8px; text-decoration:none; font-weight:bold; z-index:1000; box-shadow:0 4px 8px rgba(0,0,0,0.2); font-family:sans-serif;';
    btn.innerHTML = '<img src="https://cdn-icons-png.flaticon.com/512/124/124034.png" style="width:24px; height:24px; vertical-align:middle; margin-right:8px;"> Fale conosco';
    document.body.appendChild(btn);
})();
