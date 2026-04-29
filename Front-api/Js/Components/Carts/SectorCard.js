export const createSeat = (seat, currentSelectionId, onClick) => {
    const isSelected = seat.id === currentSelectionId;
    const isAvailable = seat.status === 'Available';
    const isReserved = seat.status === 'Reserved';
    const isSold = seat.status === 'Sold';
    const btn = document.createElement('button');
 
    let classes = "seat-btn w-8 h-8 rounded-lg text-[10px] font-bold flex items-center justify-center transition-all ";
 
    if (isSelected) {
        classes += "bg-selected text-white shadow-lg ring-2 ring-blue-300 ring-offset-1";
    } else if (isAvailable) {
        classes += "bg-available text-white hover:shadow-md";
    } else if (isReserved) {
        classes += "bg-occupied text-white opacity-50 cursor-not-allowed";
        btn.disabled = true;
    } else if (isSold) {
        classes += "bg-gray-400 text-white opacity-40 cursor-not-allowed";
        btn.disabled = true;
    }
 
    btn.className = classes;
    btn.innerText = seat.seatNumber;
 
    if (isAvailable) {
        btn.addEventListener('click', () => onClick(seat));
    }
 
    return btn;
};