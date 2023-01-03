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
    marginTop: "0.2%",
    marginBottom: "0.2%",
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
    paddingLeft: "2px"
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
    height: "100%"
}

const redDeskStyle = {
    ...deskStyle,
    backgroundColor: "red"
}

const greenDeskStyle = {
    ...deskStyle,
    backgroundColor: "green"
}

const blueDeskStyle = {
    ...deskStyle,
    backgroundColor: "blue"
}

const guestBoxStyle = {
    border: "1px dotted #444444",
    marginTop: "2%",
    marginLeft: "50%"
}

const guestRowStyle = {
    ...seatRowStyle,
    marginTop: 0,
    marginBotton: "2px",
    marginLeft: "5px",
    marginRight: "5px"
}

const guestSeatStyle = {
    ...seatStyle,
    ...aisleLeftStyle,
    width: "18%"
}

export default function SeatRow(props) {

    const { seats, rowNr } = props
    const isQuest = !!props.isQuest;

    const getSeatStyle = (voteType) => {

        switch (voteType) {
            case 0: return greenDeskStyle
            case 1: return redDeskStyle
            case 2: return blueDeskStyle
            default: return deskStyle
        }
    }

    const createSeat = (seat) => {
        return (
            <div style={ getSeatStyle(seat?.voteType) }>
                { seat?.name }
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