import React, { Component } from "react";
import "../../Content/Main.css";
import ExpenseTable from "./ExpenseTable";
import ndjsonStream from "can-ndjson-stream";
import { readlink, readSync } from "fs";

const StreamData_url = "https://cashcog.xcnt.io/stream";
const DBData_url = "https://localhost:44395/api/Expenses/";
const totalExpenses = 20;

class Main extends Component {
  /* static data for test purposes */
  /* state = [
    {
      uuid: "92b19fc6-5386-4985-bf5c-dc56c903dd23",
      description: "Itaque fugiat repellendus velit deserunt praesentium.",
      created_at: "2019-09-22T23:07:01",
      amount: 2291,
      currency: "UZS",
      approved: true,
      employee: {
        uuid: "858142ac-299a-48f0-b221-7d6de9439454",
        first_name: "Birthe",
        last_name: "Meier"
      }
    },
    {
      uuid: "92b19fc6-5386-4985-bf5c-dc56c903dd24",
      description: "Itaque fugiat repellendus velit deserunt praesentium.",
      created_at: "2018-09-22T23:07:01",
      amount: 321,
      currency: "EUR",
      approved: false,
      employee: {
        uuid: "858142ac-299a-48f0-b221-7d6de9439454",
        first_name: "Birthe",
        last_name: "Meier"
      }
    }
  ]; */

  constructor() {
    //console.log("Main constructor called");
    super();
    this.state = {
      expenses: [],
      isNotLoaded: true //parameter to make buttons disabled or enabled
    };
    this.handleApproval = this.handleApproval.bind(this);
    this.handleDelete = this.handleDelete.bind(this);
  }

  /* - Methods to communicate with both APIs - */

  //Get the data already in the DB
  async fetchGetDatabase() {
    //console.log("fetchGetDatabase method called in Main Component");

    const response = await fetch(DBData_url);
    const result = await response.json();
    const newState = Object.assign({}, this.state);
    newState.expenses = result;
    this.setState(newState);
    //console.log("Finished FetchGetDB");
  }

  // Fetch Method to get data from Xcnt stream API
  async fetchStreamData() {
    //console.log("fetchStreamData method called in Main Component");

    const response = await fetch(StreamData_url);
    const result = await ndjsonStream(response.body);
    const reader = await result.getReader();
    let i = this.state.expenses.length;
    let newExpenses = [];

    while (true) {
      let read = await reader.read();
      if (read.done || this.state.expenses.length >= totalExpenses) {
        //limit the amount of expenses
        //console.log("Stream finished!");
        this.setState({ isNotLoaded: false });
        this.fetchSeedDatabase(newExpenses); //after all the stream chunks have been transferred...
        //...the batch will be sent to the Server API and will populate the database
        return;
      }

      //adding expenses to the state

      newExpenses.push(this.updateState(read.value, i));

      i++;
    }
  }
  /*
    fetch("https://cashcog.xcnt.io/stream")
      .then(response => {
        return ndjsonStream(response.body); //ndjsonStream parses the response.body
      })
      .then(stream => {
        let reader = stream.getReader();
        let reread; //name the callback

        reader.read().then(
          (reread = result => {
            if (result.done || this.state.expenses.length >= 20) {
              //limit the amount of expenses
              //console.log("Stream finished!");
              this.fetchSeedDatabase(); //after all the stream chunks have been transferred...
              //...the batch will be sent to the Server API and will populate the database
              return;
            }

            //adding expenses to the state

            this.updateState(result.value, i);

            i++;

            return reader.read().then(reread);
          })
        );
      })
      .catch(error => {
        alert(error);
      });*/

  //Keep adding data to the state.expenses array
  updateState = (expense, index) => {
    //console.log("updateState method called in Main Component");
    const newState = Object.assign({}, this.state);
    newState.expenses.push(expense);
    newState.expenses[index].approved = false;
    this.setState(newState);
    return newState.expenses[index];
  };

  //Seed XCNT server. with initial content
  fetchSeedDatabase(newExpenses) {
    //console.log("fetchSeedDatabase method called in Main Component");
    //console.log(newExpenses);
    fetch(DBData_url, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify(newExpenses)
    })
      .then(function(response) {
        return response.json();
      })
      .catch(error => {
        alert(error);
      });
  }

  //PUT method call with the new approved value
  fetchUpdateApproval(expense) {
    //console.log("fetchUpdateApproval method called in Main Component");
    fetch(
      DBData_url +
        expense.uuid +
        `?approved=${encodeURIComponent(expense.approved)}`,
      {
        method: "PUT"
      }
    ).catch(error => {
      alert(error);
    });
  }

  //DELETE method call to remove all the approved expenses and be done with them
  async fetchDeleteExpenses(expensesToDelete) {
    //console.log("fetchDeleteExpenses method called in Main Component");
    await fetch(DBData_url, {
      method: "DELETE",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify(expensesToDelete)
    }).catch(error => {
      alert(error);
    });
  }

  /* ----------------------------------------- */

  /* Method componentWillMount will run before any new component is added */

  async componentDidMount() {
    //console.log("componentWillMount method called in Main Component");
    await this.fetchGetDatabase();
    //Get data from Xcnt stream API
    await this.fetchStreamData();
  }

  /* Method handleApproval will update the array of expenses with the new approved value and make the PUT call to update the DB */

  handleApproval = expense => {
    //console.log("handleApproval method called in Main Component");
    const newState = Object.assign({}, this.state);
    let index = 0;

    //Look for the specific index inside the Expenses array
    for (let i = 0; i < newState.expenses.length; i++) {
      if (newState.expenses[i].uuid === expense.uuid) {
        index = i;
        break;
      }
    }

    //Update the list of exepnses with the new approved value
    newState.expenses[index] = expense;

    this.setState(newState);

    //Update the approval state of the marked expense
    this.fetchUpdateApproval(newState.expenses[index]);
  };

  /* Method handleDelete will delete all the expenses marked as approved */
  async handleDelete() {
    //console.log("handleDelete method called in Main Component");
    const expensesToDelete = this.state.expenses.filter(
      exp => exp.approved === true
    );

    await this.fetchDeleteExpenses(expensesToDelete);

    let newState = Object.assign({}, this.state);
    newState.expenses = await newState.expenses.filter(exp => {
      return exp.approved === false;
    });

    this.setState(state => {
      return { expenses: newState.expenses, isNotLoaded: true };
    });

    await this.fetchStreamData();
  }

  render() {
    //console.log("Render method called in Main Component");
    //If the component's state doesn't have any expense
    return (
      <React.Fragment>
        <ExpenseTable
          class="expenseTable"
          data={this.state} //passing array of expenses to the children components
          onApproval={this.handleApproval}
        />
        <button
          class="deleteBtn"
          onClick={this.handleDelete}
          disabled={this.state.isNotLoaded}
        >
          Delete All Approved
        </button>
      </React.Fragment>
    );
  }
}

export default Main;
