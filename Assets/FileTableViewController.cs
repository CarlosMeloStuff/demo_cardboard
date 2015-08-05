﻿using UnityEngine;
using System.Collections.Generic;
using Tacticsoft;
using Tacticsoft.Examples;
using System.IO;
using DetailTableCellNS;

namespace FileTableViewControllerNS
{
    //An example implementation of a class that communicates with a TableView
    public class FileTableViewController : MonoBehaviour, ITableViewDataSource
    {
        public DetailTableCell m_cellPrefab;
        public TableView m_tableView;

		public FileInfo[] info;

        int m_numRows;
        private int m_numInstancesCreated = 0;

        private Dictionary<int, float> m_customRowHeights;

        //Register as the TableView's delegate (required) and data source (optional)
        //to receive the calls
        void Start() {
            m_customRowHeights = new Dictionary<int, float>();
            m_tableView.dataSource = this;

			string path = Directory.GetCurrentDirectory ();
			DirectoryInfo dir = new DirectoryInfo(path);
			info = dir.GetFiles("*.mp3");

			m_numRows = info.Length;
        }

        #region ITableViewDataSource

        //Will be called by the TableView to know how many rows are in this table
        public int GetNumberOfRowsForTableView(TableView tableView) {
            return m_numRows;
        }

        //Will be called by the TableView to know what is the height of each row
        public float GetHeightForRowInTableView(TableView tableView, int row) {
            return GetHeightOfRow(row);
        }

        //Will be called by the TableView when a cell needs to be created for display
        public TableViewCell GetCellForRowInTableView(TableView tableView, int row) {
            DetailTableCell cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as DetailTableCell;
            if (cell == null) {
                cell = (DetailTableCell)GameObject.Instantiate(m_cellPrefab);
                cell.name = "DynamicHeightCellInstance_" + (++m_numInstancesCreated).ToString();
                cell.onCellHeightChanged.AddListener(OnCellHeightChanged);
            }
            cell.rowNumber = row;
            cell.height = GetHeightOfRow(row);
			cell.m_rowNumberText.text = info [row].Name;
            return cell;
        }

        #endregion

        private float GetHeightOfRow(int row) {
            if (m_customRowHeights.ContainsKey(row)) {
                return m_customRowHeights[row];
            } else {
                return m_cellPrefab.height;
            }
        }

        private void OnCellHeightChanged(int row, float newHeight) {
            if (GetHeightOfRow(row) == newHeight) {
                return;
            }
            //Debug.Log(string.Format("Cell {0} height changed to {1}", row, newHeight));
            m_customRowHeights[row] = newHeight;
            m_tableView.NotifyCellDimensionsChanged(row);
        }

    }
}
