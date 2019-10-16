import React, { Component } from "react";
import "../../Content/Expense.css";

class Expense extends Component {
  constructor(props) {
    //console.log("Contructor method called in Expense Component");
    super(props);
    this.state = { checked: props.expense.approved };
  }

  /* Method function to change the approved value between true and false */
  handleChange = () => {
    //console.log("handleChange method called in Expense Component");

    //Updating component's state
    const newState = Object.assign({}, this.state);
    newState.checked = !this.state.checked; //triggering change
    this.setState(newState);

    //Updating Expense object with the new approved value
    const newExpense = Object.assign({}, this.props.expense);
    newExpense.approved = newState.checked;
    this.props.onApproval(newExpense); //calling parent method located in Main component
  };

  changeColor = () => {
    return this.state.checked === false ? "red" : "green";
  };

  render() {
    const approvalStyle = {
      color: this.changeColor(),
      fontSize: 15
    };

    //console.log("Render method called in Expense Component");

    return (
      <React.Fragment>
        <span style={approvalStyle}>&#9673;</span>
        <button
          onClick={this.handleChange}
          class="buttonExpense"
          disabled={this.props.isNotLoaded}
        >
          {this.state.checked === false ? "Approve" : "Reject"}
        </button>
      </React.Fragment>
    );
  }
}

export default Expense;
