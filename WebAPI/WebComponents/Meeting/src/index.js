import React from "react"
import ReactDom from "react-dom/client"
import Meeting from "./Meeting"

const root = ReactDom.createRoot( document.getElementById('meeting'))
root.render(
    <Meeting />
)