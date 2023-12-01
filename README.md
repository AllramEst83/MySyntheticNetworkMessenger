# MySyntheticNetworkMessenger
Fake ChatGpt powered messenging service.

## Overview

This is a web-based chat application designed to simulate conversations with different personalities. It's an interactive platform where users can create 'contacts' with various personality traits and engage in simulated chats.
This application does not use any form of data storage. All chats are stored in memory. 
This application is intended to demonstrate how you can dynamically create chatbots.
## Features

- **Contact Creation:** Users can create contacts, each with a unique set of personality traits, such as formality, politeness, humor, and confidence.
- **Chat Simulation:** The application allows for chat interactions with these created contacts, simulating a conversation based on the personality parameters set.
- **Era Selection:** Users can select the era (80s, 90s, or 00s) to which the contact's personality traits belong.
- **Dynamic Chat Tabs:** Each contact has a dedicated chat tab, which can be selected to view the conversation history.
- **Message Input Validation:** The send button is enabled or disabled based on the chat tab selection and input length.
- **Token Count Management:** The app includes logic to manage the count of tokens (a measure of conversation length) to adhere to API limits.

## Technology Stack

- **SignalR:** Used for real-time web functionality, allowing server-side code to send asynchronous notifications to client-side web applications.
- **Bootstrap & jQuery:** Utilized for modals, forms, and event handling to enhance user experience and simplify DOM manipulations.
- **.NET 7:** The backend is built using .NET 6, providing a robust framework for building high-performance web applications.
- **C#:** The primary programming language for the server-side logic.

## Development Environment Setup

### Prerequisites

- .NET 7 SDK
- Visual Studio 2022 or another compatible IDE
- Git for version control
