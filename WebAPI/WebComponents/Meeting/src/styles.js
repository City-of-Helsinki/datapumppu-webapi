const textStyles = {
    fontSize: "16px",
    fontFamily: "Verdana, Arial, sans-serif",
    color: "#414143",
    lineHeight: "1.4"
}

const iconStyle = {
    fontSize: "16px",
    paddingRight: "10px",
}

const itemStyle = {
    ...textStyles,
    backgroundColor: "white",
    padding: "15px",
    margin: "4px",
    WebkitTransition: "all 0.2s ease",
    MozTransition: "all 0.2s ease",
    OTransition: "all 0.2s ease",
    transition: "all 0.2s ease"
}

const itemOpenStyle = {
    ...itemStyle,
    backgroundColor: "#dedfe1"
}

const contentStyle = {
    ...textStyles,
    padding: "15px 0px 15px 60px"
}

const agendaButtonStyle = {
    ...textStyles,
    backgroundColor: "inherit",
    border: "none",
    fontWeight: "bold",
    textAlign: "start",
    display: "flex",
    margin: "8px 8px 6px 8px",
}
const agendaTitleStyle = {
    ...agendaButtonStyle,
    maxWidth: "70%",
}

const containerStyle = {
    width: "100%",
    display: "flex",
    maxWidth: "1140px",
}
const agendaStyle = {
    paddingTop: "35px"
}

const attachmentTable = {
    table: {
        display: "table",
        maxWidth: "70%"
    },
    row: {
        display: "table-row",

    },
    cell: {
        display: "table-cell",
        padding: "0 10px 0 0",
    }
}

const loginStyle = {
    loginPopup: {
        position: "fixed",
        top: "0%",
        right: "0%"
    },
    loginForm: {
        ...textStyles,
        position: "absolute",
        top: "0px",
        right: "0px",
        width: "300px",
        backgroundColor: "#333333",
        padding: "10px",
        color: "#ffffff",
    },
    label: {
        fontSize: "14px",
        marginBottom: "5px"
    },
    input: {
        width: "100%",
        marginBottom: "10px",
        padding: "10px",
        color: "black",
    },
    buttonRow: {
        marginTop: "10px",
        paddingTop: "10px",
        textAlign: "right",
    },
    button: {
        marginLeft: "2px",
        padding: "5px",
        color: "black",
        fontSize: "14px",
    }
}

const headerStyle = {
    header: {
        ...textStyles,
        position: "fixed",
        padding: "10px",
        backgroundColor: "black",
        color: "white",
        fontSize: "20px",
        fontWeight: "bold",
        left: "0px",
        top: "0px",
        width: "100%",
        textAlign: "center",
        display: "flex",
        flexDirection: "row",
        alignItems: "center"
    },
    logoutBtn: {
        float: "right",
        marginRight: "20px",
        color: "white",
        backgroundColor: "inherit",
        border: "none"
    }
}

const editorStyle = {
    border: "1px solid #414143",
    background: "white",
    padding: "10px",
    marginTop: "2px"
}

const linkStyle = {
    ...textStyles,
    color: "#0072c6"
}

export {
    iconStyle,
    itemStyle,
    itemOpenStyle,
    contentStyle,
    agendaButtonStyle,
    containerStyle,
    agendaStyle,
    agendaTitleStyle,
    attachmentTable,
    loginStyle,
    headerStyle,
    editorStyle,
    linkStyle
}