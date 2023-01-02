
import React, { useEffect } from 'react'
import SeatRow from './SeatRow'

const champerStyle = {
    fontSize: "65%",
    height: "60em",
    pageBreakInside: "avoid",
    backgroundColor: "#dedfe1",
    margin: 0,
    padding: 0
}

export default function SeatMap() {

    // seats for testing purposes
    const seats = [
        {},
        {
            seat: 1,
            name: "Ällikki Runo (Kok.)",
        },
        {
            seat: 2,
            name: "Ripi Mitja (Saml.)",
        },
        {
            seat: 3,
            name: "Lappa Era (Kok)",
        },        
        {
            seat: 5,
            name: "Sasa Raisu (PerusS)",
        },
        {
            seat: 6,
            name: "Mitja Viima (SFP)",
        },        
        {
            seat: 7,
            name: "Killi Sasha (Gröna)",
        },        
        {
            seat: 8,
            name: "Indigo Van (Vihr)",
        },        
        {
            seat: 9,
            name: "Oiva Hasan (SDP)",
        },        
        {
            seat: 10,
            name: "Elof Knut (SDP)",
        },        
        {
            seat: 11,
            name: "Asseri Dennis (Vänst.)",
        },        
        {
            seat: 12,
            name: "Marian Catherine",
        },        
        {
            seat: 13,
            name: "Soili Unna",
        },        

    ]

    seats[91] = {
        seat: 91,
        name: "Diana Rose (Vihr.)",
    }

    seats[101] = {
        seat: 101,
        name: "Venni Utu (Pri)",
    }

    seats[102] = {
        seat: 102,
        name: "Eeti Van",
    }

    seats[103] = {
        seat: 103,
        name: "Örri Killi (Apri)",
    }

    seats[104] = {
        seat: 104,
        name: "Nikola Jamie (Apri)",
    }

    seats[105] = {
        seat: 105,
        name: "Salad Toive (Apri)",
    }

    seats[106] = {
        seat: 106,
        name: "Raisu Ronji (Apri)",
    }

    return (
        <div style={champerStyle}>
            <SeatRow seats={seats}></SeatRow>
            <SeatRow rowNr={0} seats={seats}></SeatRow>
            <SeatRow rowNr={1} seats={seats}></SeatRow>
            <SeatRow rowNr={2} seats={seats}></SeatRow>
            <SeatRow rowNr={3} seats={seats}></SeatRow>
            <SeatRow rowNr={4} seats={seats}></SeatRow>
            <SeatRow rowNr={5} seats={seats}></SeatRow>
            <SeatRow rowNr={6} seats={seats}></SeatRow>
            <SeatRow rowNr={7} seats={seats}></SeatRow>
            <SeatRow rowNr={8} seats={seats}></SeatRow>
            <SeatRow rowNr={10} seats={seats} isQuest={true}></SeatRow>
        </div>
    )
}





