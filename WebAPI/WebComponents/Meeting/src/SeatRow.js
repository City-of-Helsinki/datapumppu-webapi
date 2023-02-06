import React from 'react'

const seatRowStyle = {
    color: "white",
    backgroundColor: "#dedfe1",
    padding: 0,
    fontFamily: "Sans-Serif",
    clear: "both",
    height: "7%",
    marginLeft: 0,
    marginRight: 0,
    marginTop: "2px",
    marginBottom: "2px",
};

const firstSeatStyle = {
    marginLeft: "1.5%"
}

const lastSeatStyle = {
    marginRight: "1.5%"
}

const seatStyle = {
    width: "8%",
    height: "100%",
    float: "left",
    position: "relative",
    backgroundColor: "#eeeeee",
    color: "#414143",
}

const aisleLeftStyle = {
    marginLeft: "0.5%",
    marginRight: "1.4%"
}

const aisleRightStyle = {
    marginLeft: "1.4%",
    marginRight: "0.5%"
}

const deskStyle = {
    backgroundColor: "#eeeeee",
    marginBottom: "5%",
    boxSizing: "border-box",
    height: "100%",
}

const redDeskStyle = {
    ...deskStyle,
    backgroundColor: "#e62224",
    color: "#ffffff"
}

const redDeskStyleBW = {
    ...deskStyle,
    backgroundColor: "#aaa",
    color: "#000"
}

const greenDeskStyle = {
    ...deskStyle,
    backgroundColor: "#64bb46",
    color: "#ffffff"
}

const greenDeskStyleBW = {
    ...deskStyle,
    backgroundColor: "#000",
    color: "#fff"
}

const blueDeskStyle = {
    ...deskStyle,
    backgroundColor: "#98d8e1",
    color: "#ffffff"
}

const blueDeskStyleBW = {
    ...deskStyle,
    backgroundColor: "#fff",
    color: "#000"
}

const absentDeskStyle = {
    ...deskStyle,
    border: "1px solid black",
}

const guestBoxStyle = {
    border: "1px dotted #444444",
    marginTop: "2%",
    marginLeft: "50%",
}

const guestRowStyle = {
    ...seatRowStyle,
    marginTop: 0,
    marginBotton: "2px",
    marginLeft: "5px",
    marginRight: "5px",
}

const guestSeatStyle = {
    ...seatStyle,
    ...aisleLeftStyle,
    width: "18%",
}

export default function SeatRow(props) {

    const { seats, rowNr, showColors, showName } = props
    const isQuest = !!props.isQuest;

    const getSeatStyle = (voteType) => {
        switch (voteType) {
            case 0: return showColors ? greenDeskStyle : greenDeskStyleBW
            case 1: return showColors ? redDeskStyle : redDeskStyleBW
            case 2: return showColors ? blueDeskStyle : blueDeskStyleBW
            case 3: return absentDeskStyle
            default: return deskStyle
        }
    }

    const createSeat = (seat) => {
        return (
            <div style={ getSeatStyle(seat?.voteType) }>
                {showName && seat?.name }
            </div>
        )
    }

    return (
        <>
        {rowNr === undefined && !!isQuest === false &&
            <div style={seatRowStyle}>
                <div style={{...seatStyle, marginLeft:"45.5%"}}>{ createSeat(seats[91]) }</div>
            </div>
        }
        {rowNr !== undefined && isQuest === false &&
            <div style={seatRowStyle}>
                <div style={{...seatStyle, ...firstSeatStyle}}>{ createSeat(seats[(rowNr * 10) + 1]) }</div>
                <div style={{...seatStyle, ...aisleLeftStyle}}>{ createSeat(seats[(rowNr * 10) + 2]) }</div>
                <div style={{...seatStyle, ...aisleRightStyle}}>{ createSeat(seats[(rowNr * 10) + 3]) }</div>
                <div style={{...seatStyle, ...aisleLeftStyle}}>{ createSeat(seats[(rowNr * 10) + 4]) }</div>
                <div style={{...seatStyle, ...aisleRightStyle}}>{ createSeat(seats[(rowNr * 10) + 5]) }</div>
                <div style={{...seatStyle, ...aisleLeftStyle}}>{ createSeat(seats[(rowNr * 10) + 6]) }</div>
                <div style={{...seatStyle, ...aisleRightStyle}}>{ createSeat(seats[(rowNr * 10) + 7]) }</div>
                <div style={{...seatStyle, ...aisleLeftStyle}}>{ createSeat(seats[(rowNr * 10) + 8]) }</div>
                <div style={{...seatStyle, ...aisleRightStyle}}>{ createSeat(seats[(rowNr * 10) + 9]) }</div>
                <div style={{...seatStyle, ...lastSeatStyle}}>{ createSeat(seats[(rowNr * 10) + 10]) }</div>
            </div>
        }
        {isQuest &&
        <div style={guestBoxStyle}>
            <div style={guestRowStyle}>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 1]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 2]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 3]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 4]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 5]) }</div>
            </div>
            <div style={guestRowStyle}>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 6]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 7]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 8]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 9]) }</div>
                <div style={guestSeatStyle}>{ createSeat(seats[(rowNr * 10) + 10]) }</div>
            </div>
        </div>
        }
        </>
    )
}