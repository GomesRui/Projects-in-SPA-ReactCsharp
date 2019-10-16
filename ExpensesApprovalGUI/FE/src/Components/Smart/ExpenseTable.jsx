import React, { Component } from "react";
import Expense from "./Expense";

import ReactTable from "react-table";
import "react-table/react-table.css";

class ExpenseTable extends Component {
  render() {
    //console.log("Render method called in ExpenseTable Component");
    /* Columns configuration for the react table */
    const columns = [
      {
        id: "uuid",
        Header: "UUID",
        accessor: "uuid"
      },
      {
        id: "description",
        Header: "Description",
        accessor: "description" // Custom value accessors!
      },
      {
        id: "created_at",
        Header: "Creation Date",
        accessor: "created_at" // Custom value accessors!
      },
      {
        id: "amount",
        Header: "Amount",
        accessor: "amount", // Custom value accessors!
        Cell: props => <span className="number">{props.value}</span>
      },
      {
        id: "currency",
        Header: "Currency",
        accessor: "currency" // Custom value accessors!
      },
      {
        id: "first_name",
        Header: "First Name",
        accessor: "employee.first_name" // Custom value accessors!
      },
      {
        id: "last_name",
        Header: "Last Name",
        accessor: "employee.last_name" // Custom value accessors!
      },
      {
        id: "approved",
        Header: "Approved",
        accessor: "approved",
        Cell: row => {
          return (
            <React.Fragment>
              <Expense
                isNotLoaded={this.props.data.isNotLoaded}
                expense={row.original} //each row's expense data will be sent to the Expense component
                onApproval={this.props.onApproval}
              />
            </React.Fragment>
          );
        }
      }
    ];
    /* ReactTable with filterable enabled */
    return (
      <ReactTable
        data={this.props.data.expenses}
        columns={columns}
        filterable
        defaultFilterMethod={(filter, row) =>
          String(row[filter.id]) === filter.value
        }
      />
    );
  }
}

export default ExpenseTable;
