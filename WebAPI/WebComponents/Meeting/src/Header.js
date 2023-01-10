import React from 'react'

export default function Header(props) {

  const style = {
    header: {
      position: "fixed",
      padding: "10px",
      backgroundColor: "black",
      color: "white",
      fontSize: "20px",
      fontWeight: "bold",
      left: "0px",
      top: "0px",
      width: "100%",
      textAlign: "center"
    },
 
    logoutBtn: {
      float: "right",
    }

  }

  return (
    <div style={style.header}>
        EDIT
      <button style={style.logoutBtn} onClick={props.submitLogout}>X</button></div>
  )
}


