# LearnMuddyBlazor

LearnMuddyBlazor is a Blazor-based project designed to help me learn and implement queue management systems and mortgage calculations in their Blazor applications.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Introduction

LearnMuddyBlazor is designed to provide a comprehensive guide for developers looking to implement queue management systems and mortgage calculations using the Blazor framework. It includes various examples and best practices to help you get started quickly.

BlazorQueue is part of this project, aimed at providing queue management functionality using the Blazor framework. It leverages the power of Blazor to create interactive and dynamic web applications.

## Features

- Real-time queue management
- Easy integration with Blazor applications
- Support for multiple queue types
- Customizable queue behavior
- Mortgage calculation with interactive UI

## Installation

To include LearnMuddyBlazor and BlazorQueue in your project, follow these steps:

1. Clone the repositories:
   ```sh
   git clone https://github.com/xMidnightGospelx/LearnMuddyBlazor.git
   git clone https://github.com/xMidnightGospelx/BlazorQueue.git
2. Set up the Azure Storage account and create a queue.

3. Configure the QueueServiceClient in your Blazor application.

## Usage
### Queue Message Manager
<details> <summary>Queue Message Manager</summary>

   ```
@page "/queue-message-manager"
@using Azure.Storage.Queues
@using Azure.Storage.Queues.Models
@inject QueueServiceClient queueServiceClient

<div class="container">
    <div class="mb-0 ">
        <label for="message" class="form-label">Message</label>
        <input type="text" class="form-control" id="message" @bind="message" />
    </div>
    <button class="btn btn-primary" @onclick="InsertMessage">Insert Message</button>
    <button class="btn btn-secondary" @onclick="RetrieveMessages">Retrieve Messages</button>
    <ul class="list-group mt-3">
        @foreach (var msg in messages)
        {
            <li class="list-group-item">@msg</li>
        }
    </ul>
</div>

@code {
    [Parameter]
    public string QueueName { get; set; }

    private string message;
    private List<string> messages = new();

    private async Task InsertMessage()
    {
        var queueClient = queueServiceClient.GetQueueClient(QueueName);
        await queueClient.SendMessageAsync(message);
        message = string.Empty;
    }

    private async Task RetrieveMessages()
    {
        var queueClient = queueServiceClient.GetQueueClient(QueueName);
        var retrievedMessages = await queueClient.ReceiveMessagesAsync(maxMessages: 10);

        messages.Clear();
        foreach (var msg in retrievedMessages.Value)
        {
            messages.Add(msg.MessageText);
        }
    }
}
```
</details>

### Program.cs
<details> <summary>Program.cs</summary>

```
using Azure.Storage.Queues;
using BlazorQueue.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(typeof(QueueServiceClient), 
    new QueueServiceClient(builder.Configuration["ConnString"])
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```
</details>

### Mortgage Calculator
<details> <summary>Mortgage Calculator</summary>

```
@page "/mortgage"

<MudGrid>
    <MudItem xs="12">
        <MudPaper Class="d-flex align-center justify-center mud-width-full py-8">
            <MudSlider TickMarks="true" Step="100000" Min="100000" Max="1000000" @bind-Value="loanAmount" />
            <MudNumericField @bind-Value="loanAmount" Step="10000" Label="Loan Amount" Variant="Variant.Outlined" />
        </MudPaper>
    </MudItem>
    <MudItem xs="12" sm="6">
        <MudPaper Class="d-flex align-center justify-center mud-width-full py-8">
            <MudNumericField @bind-Value="DecimalValueYear" Max="30" Label="Loan Years" Variant="Variant.Outlined" Step="1M" />
        </MudPaper>
    </MudItem>
    <MudItem xs="12" sm="6">
        <MudPaper Class="d-flex align-center justify-center mud-width-full py-8">
            <MudNumericField @bind-Value="DecimalValueIRate" Label="Interest Rate" Variant="Variant.Outlined" Step=".1M" />
        </MudPaper>
        <MudButton Disabled="@_processing" OnClick="FirstPayment" Variant="Variant.Filled" Color="Color.Primary">
            @if (_processing)
            {
                <MudProgressCircular Class="ms-n1" Size="Size.Large" Indeterminate="true" />
                <MudText Class="ms-2">Processing</MudText>
            }
            else
            {
                <MudText>Payment Brakedown</MudText>
            }
        </MudButton>
    </MudItem>
    <MudItem xs="6" sm="6">
        <MudPaper Class="d-flex align-center justify-space-evenly mud-width-full py-8">
            
            @if (firstPayment != null)
            {
                <MudPaper Class="d-flex align-center justify-space-evenly mud-width-full py-8">
                    <MudText Typo="Typo.h6">First Payment Details</MudText>
                    <MudText>Principal: @firstPayment.PrincipalPaid.ToString("C")</MudText>
                    <MudText>Interest: @firstPayment.InterestPaid.ToString("C")</MudText>
                    <MudText>Balance: @firstPayment.RemainingBalance.ToString("C")</MudText>
                </MudPaper>
                <MudGrid>
                    <MudItem xs="6">
                        <MudPaper Class="d-flex align-center justify-space-evenly mud-width-full py-8">
                            <MudText>First Payment: </MudText>
                            <MudText Typo="Typo.h6" >@firstSum</MudText>
                        </MudPaper>
                    </MudItem>

                    <MudChart ChartType="ChartType.Donut" Width="300px" Height="300px" InputData="@data" InputLabels="@labels">
                        <CustomGraphics>
                            <text class="donut-inner-text" x="47%" y="50%" dominant-baseline="middle" text-anchor="middle" fill="#594ae2ff" font-family="Helvetica" font-size="5">@data.Sum().ToString("C")</text>
                        </CustomGraphics>
                    </MudChart>
                </MudGrid>
            }

        </MudPaper>
    </MudItem>
    <MudItem xs="6" sm="6">
        <MudPaper Class="d-flex align-center justify-center mud-width-full py-8">
            
            @if (paymentDetails.Any())
            {
                <MudDataGrid Items="@paymentDetails" Hover="true" Bordered="true">
                    <Columns>
                        <PropertyColumn Property="x => x.Month" Title="Month" />
                        <PropertyColumn Property="x => x.PrincipalPaid" Title="Principal Paid" Format="C" />
                        <PropertyColumn Property="x => x.InterestPaid" Title="Interest Paid" Format="C" />
                        <PropertyColumn Property="x => x.RemainingBalance" Title="Remaining Balance" Format="C" />
                    </Columns>
                </MudDataGrid>
            }
            
        </MudPaper>
    </MudItem>
    <MudItem xs="6" sm="3">
        <MudPaper Class="d-flex align-center justify-center mud-width-full py-8">xs=6 sm=3</MudPaper>
    </MudItem>
    <MudItem xs="6" sm="3">
        <MudPaper Class="d-flex align-center justify-center mud-width

```
</details>

## Contributing
Feel free to contribute by submitting pull requests. Please ensure your code adheres to the coding standards and includes necessary documentation.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.
