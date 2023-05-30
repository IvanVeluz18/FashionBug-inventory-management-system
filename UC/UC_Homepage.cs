﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace InventoryMngmt.UC
{
    public partial class UC_Homepage : UserControl
    {
        function fn = new function();
        String query;
        DataSet ds;

        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ivan\Documents\InventoryDB.mdf;Integrated Security=True;Connect Timeout=30");

        DataTable dtInsert;

        public UC_Homepage()
        {
            InitializeComponent();
        }

        void populate()
        {
            try
            {
                Con.Open();
                string Myquery = "select * from StockTable";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                stockDGrid.DataSource = ds.Tables[0];
                Con.Close();
            }
            catch
            {

            }
        }

        private void UC_Homepage_Load(object sender, EventArgs e) // ORDER TAB
        {
            listBoxProd.Items.Clear();
            query = "select ProductName from StockTable where Quantity > '0'";
            ds = fn.getData(query);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listBoxProd.Items.Add(ds.Tables[0].Rows[i][0].ToString());
            }

            populate();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UC_Homepage_Load(this, null);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            listBoxProd.Items.Clear();
            query = "select ProductName from StockTable where ProductName like '" + txtSearch.Text + "%' and quantity > '0'";
            ds = fn.getData(query);

            for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listBoxProd.Items.Add(ds.Tables[0].Rows[i][0].ToString());
            }
        }

        private void listBoxProd_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtQty.Clear();

            string name = listBoxProd.GetItemText(listBoxProd.SelectedItem);

            txtProdName.Text = name;
            query = "select ID, Price from StockTable where ProductName ='" + name + "'";
            ds = fn.getData(query);

            txtProdID.Text = ds.Tables[0].Rows[0][0].ToString();
            txtPriceUnit.Text = ds.Tables[0].Rows[0][1].ToString();
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            int intparse;
            if (!int.TryParse(txtPriceUnit.Text, out intparse))
            {
                //MessageBox.Show("Input must be number", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            } else { 
                if (txtQty.Text != "")
                {
                    int uPrice = int.Parse(txtPriceUnit.Text);
                    int numUnit = int.Parse(txtQty.Text);
                    int subTotal = uPrice * numUnit;

                    txtSubtotal.Text = subTotal.ToString();
                }
                else
                {
                    txtSubtotal.Clear();
                }
            }
        }

        protected int n, totalAmt = 0;
        protected int quantity, newQuantity; //quantity = number of items ordered

        private void btnRefresh2_Click(object sender, EventArgs e)
        {
            populate();
        }

        private void columnList()
        {
            if (dtInsert == null)
            {
                dtInsert = new DataTable();
                DataColumnCollection cols = dtInsert.Columns;
                if (!cols.Contains("Product ID"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Product ID", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Product Name"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Product Name", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Quantity"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Quantity", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Price"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Price", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Subtotal"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Subtotal", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Order Date"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Order Date", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Customer"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Customer", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }

                if (!cols.Contains("Order ID"))
                {
                    DataColumn col_insertproduct_id = new DataColumn("Order ID", typeof(string));
                    dtInsert.Columns.Add(col_insertproduct_id);
                }
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            columnList();
            if (txtProdID.Text != "")
            {
                query = "select Quantity from StockTable where ID = '" + txtProdID.Text + "'";
                ds = fn.getData(query);

                quantity = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                newQuantity = quantity - int.Parse(txtQty.Text);

                if(newQuantity >= 0)
                {
                    //n = ordersDGrid.Rows.Add();
                    DataRow dr = dtInsert.NewRow();
                    
                    dr[dtInsert.Columns[0]] = txtProdID.Text;
                    dr[dtInsert.Columns[1]] = txtProdName.Text;
                    dr[dtInsert.Columns[2]] = txtPriceUnit.Text;
                    dr[dtInsert.Columns[3]] = txtQty.Text;
                    dr[dtInsert.Columns[4]] = txtSubtotal.Text;
                    dr[dtInsert.Columns[5]] = dateTimePicker.Text;
                    dr[dtInsert.Columns[6]] = txtCustName.Text;
                    dr[dtInsert.Columns[7]] = txtOrderID.Text;

                    dtInsert.Rows.Add(dr);

                    ordersDGrid.DataSource = dtInsert;
                    ordersDGrid.Refresh();

                    totalAmt = totalAmt + int.Parse(txtSubtotal.Text);
                    lblTotal.Text = "Php " + totalAmt.ToString();

                    query = "update StockTable set Quantity ='" + newQuantity + "' where ID = '" + txtProdID.Text + "'";
                    fn.setData(query, "Product Added");
                }
                else
                {
                    MessageBox.Show("Item is Out of Stock\n Only " +quantity+ " Left", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                clearAll();
                UC_Homepage_Load(this, null);
            }
            else
            {
                MessageBox.Show("Select Product First", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        int valueAmt;
        string valueId;
        protected int qty; //number of unit

        private void ordersDGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                valueAmt = int.Parse(ordersDGrid.Rows[e.RowIndex].Cells[4].Value.ToString());
                valueId = ordersDGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                qty = int.Parse(ordersDGrid.Rows[e.RowIndex].Cells[3].Value.ToString());
            }
            catch(Exception)
            {

            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(valueId != null)
            {
                try
                {
                    ordersDGrid.Rows.RemoveAt(this.ordersDGrid.SelectedRows[0].Index);
                }
                catch
                {

                }
                finally
                {
                    query = "select Quantity from StockTable where ID ='" + valueId + "'";
                    ds = fn.getData(query);
                    quantity = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    newQuantity = quantity + qty;

                    query = "update StockTable set Quantity ='" + newQuantity + "' where ID='" + valueId + "'";
                    fn.setData(query, "Product Removed from List");
                    totalAmt = totalAmt - valueAmt;

                    lblTotal.Text = "Php " + totalAmt.ToString();
                }
                UC_Homepage_Load(this, null);
            }
        }

        private void textTransact_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPriceUnit_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProdID_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnOrdr_Click(object sender, EventArgs e)
        {
            try
            {
                Con.Open();
                SqlCommand cmd;

                //cmd = new SqlCommand("insert into BreakdownTable (ProductID, ProductName, Price, Quantity, Subtotal, OrderDate, CustomerName) values (@ProductID, @ProductName, @Price, @Quantity, @Subtotal, @OrderDate, @CustomerName)", Con);
                string command = "";
                for (int i = 0; i < dtInsert.Rows.Count; i++)
                {
                    command = command + "insert into BreakdownTable (ProductID, ProductName, Price, Quantity, Subtotal, OrderDate, CustomerName, OrderID) values ('" + dtInsert.Rows[i][0].ToString() + "', '" + dtInsert.Rows[i][1].ToString() + "', '" + dtInsert.Rows[i][2].ToString() + "', '" + dtInsert.Rows[i][3].ToString() + "', '" + dtInsert.Rows[i][4].ToString() + "', '" + dtInsert.Rows[i][5].ToString() + "', '" + dtInsert.Rows[i][6].ToString() + "', '" + dtInsert.Rows[i][7].ToString() + "'); ";

                }
                cmd = new SqlCommand(command, Con);
                cmd.ExecuteNonQuery();

                Con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MessageBox.Show("Products Successfuly Added");
                totalAmt = 0;
                lblTotal.Text = "Php ";
                dtInsert.Dispose();
                ordersDGrid.DataSource = null;
                ordersDGrid.Refresh();
            }
        }

        private void clearAll()
        {
            txtProdID.Clear();
            txtProdName.Clear();
            txtPriceUnit.Clear();
            txtQty.Clear();
            txtSubtotal.Clear();
            dateTimePicker.ResetText();
            txtCustName.Clear();
            txtOrderID.Clear();
        }
    }
}
