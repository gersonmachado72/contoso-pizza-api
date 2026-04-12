# 🍕 ContosoPizza API

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Render](https://img.shields.io/badge/Deployed%20on-Render-green.svg)](https://render.com)

Uma API Web completa para gerenciamento de pizzaria com sistema de pedidos, desenvolvida com ASP.NET Core seguindo as melhores práticas da Microsoft.

## ✨ Funcionalidades

### 🍕 Cardápio Dinâmico
- Listagem de 10 sabores de pizza
- Informações de preço e se é vegetariana
- API RESTful para consulta de pizzas

### 📝 Sistema de Pedidos
- Formulário completo com nome, endereço e telefone
- Seleção de múltiplas pizzas por pedido
- Opções de tamanho (Pequena, Média, Grande, Família)
- Controle de quantidade por item
- Cálculo automático de subtotal e total

### 📊 Painel Administrativo
- Visualização de todos os pedidos realizados
- Atualização de status do pedido:
  - 🔧 Preparando
  - 🚚 Saiu para entrega
  - ✅ Finalizado
- Acompanhamento em tempo real

### 🎨 Front-end Moderno
- Layout responsivo com CSS Grid/Flexbox
- Imagens personalizadas (cabeçalho, logo, fundo)
- Efeito de blur e transparência
- Cards animados para exibição do cardápio

### 🔧 API RESTful
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/pizza` | Lista todas as pizzas |
| GET | `/pizza/{id}` | Busca pizza por ID |
| POST | `/pizza` | Adiciona nova pizza |
| PUT | `/pizza/{id}` | Atualiza pizza |
| DELETE | `/pizza/{id}` | Remove pizza |

## 🏗️ Arquitetura e Padrões

### Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|------------|--------|------------|
| ASP.NET Core | 8.0 | Framework principal |
| C# | 12.0 | Linguagem de programação |
| HTML5/CSS3 | - | Front-end |
| JavaScript (ES6) | - | Interatividade |

### Padrões de Design

- **MVC (Model-View-Controller)**: Separação clara entre dados, interface e controle
- **RESTful API**: Design de API seguindo princípios REST
- **DTO/ViewModel**: Separação entre modelos de dados e visualização

### Estrutura do Projeto
ContosoPizza/
├── Controllers/
│ ├── HomeController.cs # Páginas do site
│ ├── PizzaController.cs # API de pizzas
│ └── WeatherForecastController.cs
├── Models/
│ ├── Pizza.cs # Modelo de pizza
│ └── Pedido.cs # Modelo de pedido
├── Services/
│ ├── PizzaService.cs # Lógica de negócio das pizzas
│ └── PedidoService.cs # Lógica de negócio dos pedidos
├── Views/
│ └── Home/
│ ├── Index.cshtml # Página principal
│ ├── AdminPedidos.cshtml # Painel admin
│ └── PedidoConfirmado.cshtml # Confirmação
├── wwwroot/
│ ├── css/style.css # Estilos
│ ├── js/pedido.js # JavaScript
│ └── images/ # Imagens do site
└── Program.cs # Ponto de entrada

text

## 🚀 Como Executar Localmente

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Git
- Navegador moderno

### Passos

```bash
# Clone o repositório
git clone https://github.com/gersonmachado72/contoso-pizza-api.git
cd contoso-pizza-api

# Restaurar pacotes
dotnet restore

# Compilar
dotnet build

# Executar
dotnet run

# Acessar no navegador
# Front-end: http://localhost:5189/Home/Index
# API: http://localhost:5189/pizza
🌐 Deploy
A aplicação está configurada para deploy no Render (plano gratuito):

Push do código para o GitHub

Conectar repositório ao Render

Deploy automático a cada push

📋 Testando a API
Usando curl
bash
# Listar todas as pizzas
curl http://localhost:5189/pizza

# Buscar pizza por ID
curl http://localhost:5189/pizza/1

# Criar nova pizza
curl -X POST http://localhost:5189/pizza \
  -H "Content-Type: application/json" \
  -d '{"name":"Calabresa","price":13.99,"isVegetarian":false}'
🎯 Funcionalidades Implementadas
CRUD completo de pizzas

Sistema de pedidos com múltiplos itens

Painel administrativo

Status de pedidos (Preparando, Saiu para entrega, Finalizado)

Layout responsivo com imagens personalizadas

API RESTful documentada

Validação de dados

🤝 Contribuindo
Faça um fork do projeto

Crie uma branch para sua feature (git checkout -b feature/nova-funcionalidade)

Commit suas mudanças (git commit -m 'Adiciona nova funcionalidade')

Push para a branch (git push origin feature/nova-funcionalidade)

Abra um Pull Request

📄 Licença
Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

👨‍💻 Autor
Gerson Machado

GitHub: @gersonmachado72

Desenvolvido com 🍕 e ☕ seguindo as melhores práticas do ASP.NET Core
