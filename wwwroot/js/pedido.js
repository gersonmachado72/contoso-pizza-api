let itemCounter = 0;

function adicionarItem() {
    fetch('/pizza')
        .then(res => res.json())
        .then(pizzas => {
            const itemId = Date.now();
            
            const itemDiv = document.createElement('div');
            itemDiv.className = 'item-pedido';
            itemDiv.id = `item-${itemId}`;
            
            // Select de sabores
            const saborSelect = document.createElement('select');
            saborSelect.id = `sabor-${itemId}`;
            saborSelect.innerHTML = '<option value="">Selecione o sabor</option>' + 
                pizzas.map(p => `<option value="${p.name}" data-preco="${p.price}">${p.name} - R$ ${p.price.toFixed(2)}</option>`).join('');
            
            // Select de tamanhos
            const tamanhoSelect = document.createElement('select');
            tamanhoSelect.id = `tamanho-${itemId}`;
            tamanhoSelect.innerHTML = `
                <option value="Pequena">🍕 Pequena (R$ +0)</option>
                <option value="Média">🍕 Média (R$ +5)</option>
                <option value="Grande">🍕 Grande (R$ +10)</option>
                <option value="Família">🍕 Família (R$ +15)</option>
            `;
            
            // Input quantidade
            const qtdInput = document.createElement('input');
            qtdInput.type = 'number';
            qtdInput.min = 1;
            qtdInput.max = 10;
            qtdInput.value = 1;
            qtdInput.style.width = '70px';
            
            // Botão remover
            const removeBtn = document.createElement('button');
            removeBtn.textContent = '✖ Remover';
            removeBtn.className = 'btn-remover';
            removeBtn.onclick = () => itemDiv.remove();
            
            itemDiv.innerHTML = '<strong>🍕 Pizza:</strong><br>';
            itemDiv.appendChild(saborSelect);
            itemDiv.appendChild(document.createTextNode(' Tamanho: '));
            itemDiv.appendChild(tamanhoSelect);
            itemDiv.appendChild(document.createTextNode(' Quantidade: '));
            itemDiv.appendChild(qtdInput);
            itemDiv.appendChild(removeBtn);
            
            document.getElementById('itensPedido').appendChild(itemDiv);
        });
}

document.getElementById('pedidoForm').addEventListener('submit', function(e) {
    e.preventDefault();
    
    const itensColetados = [];
    const itemsDivs = document.querySelectorAll('.item-pedido');
    
    if (itemsDivs.length === 0) {
        alert('Adicione pelo menos uma pizza ao pedido!');
        return;
    }
    
    itemsDivs.forEach(item => {
        const selects = item.querySelectorAll('select');
        const sabor = selects[0]?.value;
        const tamanho = selects[1]?.value;
        const quantidade = item.querySelector('input[type="number"]')?.value;
        
        if (sabor && tamanho && quantidade > 0) {
            itensColetados.push({ 
                sabor: sabor, 
                tamanho: tamanho, 
                quantidade: parseInt(quantidade) 
            });
        }
    });
    
    if (itensColetados.length === 0) {
        alert('Preencha todos os itens do pedido corretamente.');
        return;
    }
    
    const pedido = {
        nomeCliente: document.getElementById('nomeCliente').value,
        endereco: document.getElementById('endereco').value,
        telefone: document.getElementById('telefone').value,
        itens: itensColetados
    };
    
    if (!pedido.nomeCliente || !pedido.endereco || !pedido.telefone) {
        alert('Preencha todos os dados do cliente!');
        return;
    }
    
    fetch('/Home/FazerPedido', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(pedido)
    })
    .then(res => res.text())
    .then(html => {
        document.body.innerHTML = html;
    })
    .catch(error => {
        alert('Erro ao enviar pedido. Tente novamente.');
    });
});
