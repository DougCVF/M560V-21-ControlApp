-- Table: Parts
CREATE TABLE IF NOT EXISTS Parts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PartNumber TEXT NOT NULL UNIQUE,
    StockWidthX REAL NOT NULL,
    StockDepthY REAL NOT NULL,
    StockHeightZ REAL NOT NULL,
    Op10PickX REAL DEFAULT 0,
    Op10PickY REAL DEFAULT 0,
    Op10PickZ REAL DEFAULT 0,
    Op20PickX REAL DEFAULT 0,
    Op20PickY REAL DEFAULT 0,
    Op20PickZ REAL DEFAULT 0,
    Op10FinX REAL DEFAULT 0,
    Op10FinY REAL DEFAULT 0,
    Op10FinZ REAL DEFAULT 0,
    Op20FinX REAL DEFAULT 0,
    Op20FinY REAL DEFAULT 0,
    Op20FinZ REAL DEFAULT 0,
    Op10VisePSI REAL DEFAULT 0,
    Op20VisePSI REAL DEFAULT 0,
    Op10Program TEXT,
    Op20Program TEXT,
    Op10CycleTime REAL DEFAULT 0.00,   -- Added field
    Op20CycleTime REAL DEFAULT 0.00    -- Added field
);

-- Table: TrayRules
CREATE TABLE IF NOT EXISTS TrayRules (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    MinWidth REAL NOT NULL,
    MaxWidth REAL NOT NULL,
    Depth REAL NOT NULL,
    Height REAL NOT NULL,
    Slots INTEGER NOT NULL
);

-- Table: RackRows
CREATE TABLE IF NOT EXISTS RackRows (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    RackNumber INTEGER NOT NULL,
    RowNumber INTEGER NOT NULL,
    AssignedPartId INTEGER,
    TrayTypeId INTEGER,
    Active INTEGER DEFAULT 1,
    FOREIGN KEY (AssignedPartId) REFERENCES Parts(Id),
    FOREIGN KEY (TrayTypeId) REFERENCES TrayRules(Id)
);

INSERT INTO TrayRules (Name, MinWidth, MaxWidth, Depth, Height, Slots)
VALUES 
('Small', 1.0, 3.0, 10.0, 2.5, 10),
('Medium', 3.0, 6.0, 10.0, 2.5, 5),
('Large', 6.0, 8.0, 10.0, 2.5, 4);
