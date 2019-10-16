import React, { Component } from "react";
import "./Content/App.css";
import Header from "./Components/Dumb/Header";
import Main from "./Components/Smart/Main";
import Footer from "./Components/Dumb/Footer";

class App extends Component {
  render() {
    return (
      <div>
        <Header />
        <Main />
        <Footer />
      </div>
    );
  }
}

export default App;
