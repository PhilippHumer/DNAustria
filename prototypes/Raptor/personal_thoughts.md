pros:

- it's very easy and fast to get an application that does something

- it's very easy to adapt the current implementation via prompting. For example, I the AI implemented a Address object, but the frontend expected an Organization object instead. So i just wrote a few lines as a prompt "Change the Address model with the same data to an Organization model" and the AI did all the changes from db changes to code changes and also adapt the tests

- the AI Integration in VS Code Copilot knows the context of the app and so you can easily create some docker integration for example 



cons:

- there are some limitation to the request you can send to the AI agent. If you run into those limitation during implementation phase it is really annoying because you need to check at what stage the current implementation is and need to adapt or finish the implementation yourself

- some decisions regarding the implementation are not really good for example that you have an organization with street, postal_code, city, state and an address elements which is just a string containing all those elmements. From my point of view, there is no need for this element
